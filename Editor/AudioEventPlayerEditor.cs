using UnityEditor;
using UnityEngine;

namespace JauntyBear.UnityAudioEvents
{
    [CustomEditor(typeof(AudioEventPlayer),true)]
    public class AudioEventPlayerEditor : Editor
    {
        private AudioEventPlayer _target;
        [SerializeField] private AudioSource _previewer;

        public void OnEnable()
        {
            _target = (AudioEventPlayer)target;
            _previewer = EditorUtility.CreateGameObjectWithHideFlags("Audio preview", HideFlags.HideAndDontSave, typeof(AudioSource)).GetComponent<AudioSource>();
        }

        public void OnDisable()
        {
            DestroyImmediate(_previewer.gameObject);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Play"))
            {
                _target.Play();
            }
        }
    }
}