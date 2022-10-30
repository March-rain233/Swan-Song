using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static GameToolKit.Dialog.TextEffectProcessor;

namespace GameToolKit.Dialog
{
    /// <summary>
    /// �ı�Ч������
    /// </summary>
    public class TextEffectTask
    {
        TextEffectProcessor _processor;
        TextMeshProUGUI _textController;

        RichTextTree _tree;
        string _rawText;

        /// <summary>
        /// �Ƿ���Կհ��ַ��ĵȴ�ʱ��
        /// </summary>
        public bool IsIgnoreWhiteSpaceWaiting = true;

        /// <summary>
        /// �ı���ǰ�ٶ� �ַ�/��
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
        /// �ı�Ĭ���ٶ�
        /// </summary>
        public float DefaultSpeed = 4;

        /// <summary>
        /// �ı��ٶ�ջ
        /// </summary>
        public Stack<float> SpeedStack { get; private set; } = new Stack<float>();

        /// <summary>
        /// �����ȡ��һ���ַ�����Ҫ��ʱ��
        /// </summary>
        public float NextCharacterTime;

        /// <summary>
        /// ��ȫ�����ı�����
        /// </summary>
        public event Action Completed;

        Dictionary<string, TagProcessor> _tagProcessors => _processor.TagProcessors;

        Queue<PairTagNode> _begin;
        Queue<PairTagNode> _end;
        Queue<EmptyTagNode> _empty;
        IEnumerator<(Queue<PairTagNode> begin, Queue<PairTagNode> end, Queue<EmptyTagNode> emptys)> _traver;

        /// <summary>
        /// ��ǰ�����Э��
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

            //todo�������Ժ�Ҫ�и�Э�̹�����������
            coroutine = textMesh.StartCoroutine(Process());
        }

        /// <summary>
        /// ������ɻص�
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        public TextEffectTask OnComplete(Action callback)
        {
            Completed += callback;
            return this;
        }

        /// <summary>
        /// ����Ĭ���ٶ�
        /// </summary>
        /// <param name="speed"></param>
        /// <returns></returns>
        public TextEffectTask SetDefaultSpeed(float speed)
        {
            DefaultSpeed = speed;
            return this;
        }

        /// <summary>
        /// ��ʾȫ���ı�
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
        /// ǿ�ƹر�����
        /// </summary>
        public void EndTask()
        {
            _textController.StopCoroutine(coroutine);
        }

        /// <summary>
        /// �����ı�
        /// </summary>
        /// <returns></returns>
        IEnumerator Process()
        {
            //ÿһ֡������Ч�����и���
            while (true)
            {
                _textController.ForceMeshUpdate();

                //�ı���Ϣ
                var textInfo = _textController.textInfo;
                //��ÿ���ֽ��и���
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

                //Ӧ�ø���
                _textController.UpdateVertexData();

                //�ı�����ȡ
                if (!IsComplete)
                {
                    if (NextCharacterTime <= 0)
                    {
                        if (_empty.Count > 0) //������ڻ�δ����Ŀձ�ǩ���������ձ�ǩ
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
                        else //���������һ�����ı�
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
                                if (_empty.Count == 0) //���ձ�ǩ��ĿΪ0ʱ˵�����������ʾ�ַ�
                                {
                                    ++_textController.maxVisibleCharacters;
                                    //���������Կհ��ַ�ʱ����������Ŀո�
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
