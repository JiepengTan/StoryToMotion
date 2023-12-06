using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public class PlayCustomPlayable : MonoBehaviour
{
    [Range(0, 1)] public float weight;
    public AnimationClip clip1;
    public AnimationClip clip2;

    PlayableGraph m_graph;
    AnimationBlendPlayableBehaviour m_blendPlayableBehaviour;

    void Start()
    {
        m_graph = PlayableGraph.Create("ChanPlayableGraph");
        var animationOutputPlayable = AnimationPlayableOutput.Create(m_graph, "AnimationOutput", GetComponent<Animator>());
        var blendPlayable = ScriptPlayable<AnimationBlendPlayableBehaviour>.Create(m_graph, 1);
        m_blendPlayableBehaviour = blendPlayable.GetBehaviour();
        m_blendPlayableBehaviour.Init(clip1, clip2,1);
        animationOutputPlayable.SetSourcePlayable(blendPlayable);
        m_graph.Play();
    }

    private void Update() {
        m_blendPlayableBehaviour.firstClipWeight = weight;
    }

    void OnDestroy() {
        m_graph.Destroy();
    }
}