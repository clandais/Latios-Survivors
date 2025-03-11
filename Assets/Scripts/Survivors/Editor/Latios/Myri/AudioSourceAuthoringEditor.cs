using Latios.Myri.Authoring;
using UnityEditor;
using UnityEngine;

namespace Latios.Myri.Editor
{
    [CustomEditor(typeof(AudioSourceAuthoring))]
    [CanEditMultipleObjects]
    public class AudioSourceAuthoringEditor : UnityEditor.Editor
    {
        AudioSourceAuthoring _source;
        
        void OnSceneGUI()
        {
             _source = target as AudioSourceAuthoring;
             if (!_source) return;
            InnerHandles();
        }


        void InnerHandles()
        {
            var transform = _source.transform;
            var position  = transform.position;
            
            
            Color color = Handles.color;
            
            
            

            if (!_source.useCone)

            {
                EditorGUI.BeginChangeCheck();

                Handles.color = new Color(0.5f, 0.7f, 1f, 0.5f);
                float n = Handles.RadiusHandle(Quaternion.identity, position, _source.innerRange, false);
                Handles.color = new Color(0.5f, 0.7f, 1f, 0.33f);
                float n2 = Handles.RadiusHandle(Quaternion.identity, position, _source.outerRange, false);

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject((UnityEngine.Object)target, "AudioSourceAuthoring Distance");
                    _source.innerRange = n;
                    _source.outerRange = n2;
                }
            }
            else
            {
                EditorGUI.BeginChangeCheck();
                Handles.color = new Color(0.5f, 0.7f, 1f, 0.5f);
                Handles.DrawWireDisc(position, transform.forward, _source.innerAngle);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject((UnityEngine.Object)target, "AudioSourceAuthoring Distance");
   
                }
            }
            
            Handles.color = color;
        }
    }
}