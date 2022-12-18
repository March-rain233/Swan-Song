using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameToolKit;

public class ArtifactSetting : ScriptableSingleton<ArtifactSetting>
{
    public List<Artifact> Artifacts = new();

    public IEnumerable<Artifact> RemainArtifacts => Artifacts.Except(GameManager.Instance.GameData.Artifacts);
}
