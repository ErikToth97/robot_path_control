using UnityEngine;
using System.Collections;


namespace TMPro.Examples
{
    
    public class BasicDummyMove : MonoBehaviour
    {

        private TextMeshPro m_textMeshPro;
        public Vector3 startPos;
        public Vector3 endPos;
        //private TMP_FontAsset m_FontAsset;

        private const string label = "The <#0050FF>count is: </color>{0:2}";
        private float m_frame;


        void Start()
        {
            startPos = gameObject.transform.localPosition;
            endPos = new Vector3(-5.0f, -0.5f, 10.85f);
        }


        void Update()
        {
            var currentPos = gameObject.transform.localPosition;
            if(currentPos.x <= endPos.x)
            {
                gameObject.transform.localPosition = startPos;
            }

        }

    }
}
