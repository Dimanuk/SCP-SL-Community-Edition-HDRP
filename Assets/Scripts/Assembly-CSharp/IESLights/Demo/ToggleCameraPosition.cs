namespace IESLights.Demo
{
    public class ToggleCameraPosition : global::UnityEngine.MonoBehaviour
    {
        public global::System.Collections.Generic.List<global::UnityEngine.Transform> Positions;

        private int _positionIndex;

        private void Start()
        {
            base.transform.position = Positions[_positionIndex].position;
            base.transform.rotation = Positions[_positionIndex].rotation;
        }

        private void Update()
        {
            if (global::UnityEngine.Input.GetKeyDown(global::UnityEngine.KeyCode.Tab))
            {
                _positionIndex++;
                _positionIndex %= Positions.Count;
            }
            base.transform.position = Positions[_positionIndex].position;
            base.transform.rotation = Positions[_positionIndex].rotation;
        }
    }
}
