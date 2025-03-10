using System;
using Latios.Myri.Authoring;
using UnityEditor;
using UnityEngine;

namespace Survivors.Play.Components.Debug
{
    
    public class MyriAudioSourceGizmos : AudioSourceAuthoring
    {
        void OnDrawGizmosSelected()
        {
            // display an orange disc representing the inner range of the audio source
            var color = new Color(1, 0.8f, 0.4f, 1);
            Handles.color = color;
            Handles.DrawWireDisc(transform.position, transform.up, innerRange);
            // display object "value" in scene
            GUI.color = color;
            Handles.Label(transform.position + Vector3.right*innerRange/2f, innerRange.ToString("F1"));
            
            // display a red disc representing the outer range of the audio source
            var color2 = new Color(1, 0.7f, 0.3f, 1);
            Handles.color = color2;
            Handles.DrawWireDisc(transform.position, transform.up, outerRange);
            // display object "value" in scene
            GUI.color = color2;
            Handles.Label(transform.position + Vector3.right*outerRange/2f, outerRange.ToString("F1"));
            
        }
    }
}
