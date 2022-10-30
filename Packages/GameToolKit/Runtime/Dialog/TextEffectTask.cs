using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static GameToolKit.Dialog.TextEffectProcessor;

namespace GameToolKit.Dialog
{
    /// <summary>
    /// 文本效果任务
    /// </summary>
    public class TextEffectTask
    {
        TextEffectProcessor _processor;
        TextMeshProUGUI _textController;

        RichTextTree _tree;
        string _rawText;

        /// <summary>
        /// 是否忽略空白字符的等待时间
        /// </summary>
        public bool IsIgnoreWhiteSpaceWaiting = true;

        /// <summary>
        /// 文本当前速度 字符/秒
        /// </summary>
        public float TextSpeed
        {
            get
            {
                if (SpeedStack.Count > 0)
                {
                    return SpeedStack.Peek();
                }
                return DefaultSpeed;
            }
        }

        /// <summary>
        /// 文本默认速度
        /// </summary>
        public float DefaultSpeed = 4;

        /// <summary>
        /// 文本速度栈
        /// </summary>
        public Stack<float> SpeedStack { get; private set; } = new Stack<float>();

        /// <summary>
        /// 距离读取下一个字符所需要的时间
        /// </summary>
        public float NextCharacterTime;

        /// <summary>
        /// 当全部的文本可视
        /// </summary>
        public event Action Completed;

        Dictionary<string, TagProcessor> _tagProcessors => _processor.TagProcessors;

        Queue<PairTagNode> _begin;
        Queue<PairTagNode> _end;
        Queue<EmptyTagNode> _empty;
        IEnumerator<(Queue<PairTagNode> begin, Queue<PairTagNode> end, Queue<EmptyTagNode> emptys)> _traver;

        /// <summary>
        /// 当前任务的协程
        /// </summary>
        Coroutine coroutine;

        public bool IsComplete { get; private set; }

        public TextEffectTask(TextEffectProcessor processor, TextMeshProUGUI textMesh, string rawText)
        {
            _textController = textMesh;
            _processor = processor;
            _rawText = rawText;

            _tree = new RichTextTree(rawText);
            _begin = new Queue<PairTagNode>();
            _end = new Queue<PairTagNode>();
            _empty = new Queue<EmptyTagNode>();
            _traver = RichTextHelper.GetTagInTraver(_tree);
            IsComplete = false;

            _textController.SetText(RichTextHelper.GetOutPutText(_tree));
            _textController.maxVisibleCharacters = 0;
            NextCharacterTime = 0;

            //todo：或许以后要有个协程管理器来启动
            coroutine = textMesh.StartCoroutine(Process());
        }

        /// <summary>
        /// 设置完成回调
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        public TextEffectTask OnComplete(Action callback)
        {
            Completed += callback;
            return this;
        }

        /// <summary>
        /// 设置默认速度
        /// </summary>
        /// <param name="speed"></param>
        /// <returns></returns>
        public TextEffectTask SetDefaultSpeed(float speed)
        {
            DefaultSpeed = speed;
            return this;
        }

        /// <summary>
        /// 显示全部文本
        /// </summary>
        public TextEffectTask DisplayAll()
        {
            IsComplete = true;
            _textController.maxVisibleCharacters = _textController.textInfo.characterCount;
            NextCharacterTime = 0;
            Action solveTag = () =>
            {
                while (_begin.Count > 0)
                {
                    var tag = _begin.Dequeue();
                    if (_tagProcessors.TryGetValue(tag.Tag, out var processor))
                    {
                        processor.OnTagInvoke?.Invoke(new TagContext()
                        {
                            Tag = tag,
                            Target = this
                        });
                    }
                }
                while (_end.Count > 0)
                {
                    var tag = _end.Dequeue();
                    if (_tagProcessors.TryGetValue(tag.Tag, out var processor))
                    {
                        processor.OnTagCancel?.Invoke(new TagContext()
                        {
                            Tag = tag,
                            Target = this
                        });
                    }
                }
                while (_empty.Count > 0)
                {
                    var tag = _empty.Dequeue();
                    if (_tagProcessors.TryGetValue(tag.Tag, out var processor))
                    {
                        processor.OnTagInvoke?.Invoke(new TagContext()
                        {
                            Tag = tag,
                            Target = this
                        });
                    }
                }
            };
            solveTag();
            while (_traver.MoveNext())
            {
                (_begin, _end, _empty) = _traver.Current;
                solveTag();
            }

            Completed?.Invoke();
            return this;
        }

        /// <summary>
        /// 强制关闭任务
        /// </summary>
        public void EndTask()
        {
            _textController.StopCoroutine(coroutine);
        }

        /// <summary>
        /// 处理文本
        /// </summary>
        /// <returns></returns>
        IEnumerator Process()
        {
            //每一帧对字体效果进行更新
            while (true)
            {
                _textController.ForceMeshUpdate();

                //文本信息
                var textInfo = _textController.textInfo;
                //对每个字进行更新
                for (int i = 0; i < _textController.maxVisibleCharacters; i++)
                {
                    var pairList = RichTextHelper.GetNonNativeTagsOfIndex(_tree, i);
                    foreach (var tag in pairList)
                    {
                        if (_tagProcessors.TryGetValue(tag.tag.Tag, out TagProcessor processor))
                        {
                            processor.OnRenderCharacter?.Invoke(new TagRenderContext()
                            {
                                Target = this,
                                Tag = tag.tag,
                                TextInfo = textInfo,
                                Index = i,
                                IndexInTag = tag.index
                            });
                        }
                    }
                }

                //应用更改
                _textController.UpdateVertexData();

                //文本流读取
                if (!IsComplete)
                {
                    if (NextCharacterTime <= 0)
                    {
                        if (_empty.Count > 0) //如果存在还未处理的空标签则继续处理空标签
                        {
                            while (NextCharacterTime <= 0 && _empty.Count > 0)
                            {
                                var tag = _empty.Dequeue();
                                if (_tagProcessors.TryGetValue(tag.Tag, out var processor))
                                {
                                    processor.OnTagInvoke?.Invoke(new TagContext()
                                    {
                                        Tag = tag,
                                        Target = this
                                    });
                                }
                            }
                        }
                        else //负责读入下一步的文本
                        {
                            if (_traver.MoveNext())
                            {
                                (_begin, _end, _empty) = _traver.Current;
                                while (_begin.Count > 0)
                                {
                                    var tag = _begin.Dequeue();
                                    if (_tagProcessors.TryGetValue(tag.Tag, out var processor))
                                    {
                                        processor.OnTagInvoke?.Invoke(new TagContext()
                                        {
                                            Tag = tag,
                                            Target = this
                                        });
                                    }
                                }
                                while (_end.Count > 0)
                                {
                                    var tag = _end.Dequeue();
                                    if (_tagProcessors.TryGetValue(tag.Tag, out var processor))
                                    {
                                        processor.OnTagCancel?.Invoke(new TagContext()
                                        {
                                            Tag = tag,
                                            Target = this
                                        });
                                    }
                                }
                                if (_empty.Count == 0) //当空标签数目为0时说明读入的是显示字符
                                {
                                    ++_textController.maxVisibleCharacters;
                                    //当开启忽略空白字符时，跳过读入的空格
                                    if(!IsIgnoreWhiteSpaceWaiting ||
                                        !char.IsWhiteSpace(textInfo.characterInfo[_textController.maxVisibleCharacters - 1].character))
                                    {
                                        NextCharacterTime = SpeedToTime(TextSpeed);
                                    }
                                }
                            }
                            else
                            {
                                IsComplete = true;
                                Completed?.Invoke();
                            }
                        }
                    }
                    else
                    {
                        NextCharacterTime -= Time.deltaTime;
                    }
                }

                yield return null;
            }
        }

        float SpeedToTime(float speed)
        {
            return 1 / speed;
        }
    }
}
