using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TechXR.Core.Sense
{
    /// <summary>
    /// Teleport functionality main class
    /// </summary>
    internal class Teleporter : MonoBehaviour
    {
        #region PUBLIC_MEMBERS
        /// <summary>
        /// Visual marker to display the ground position
        /// </summary>
        public GameObject PositionMarker;
        /// <summary>
        /// Dots for displaying the teleporter ray
        /// </summary>
        public GameObject Dot;
        /// <summary>
        /// Player object to teleport
        /// </summary>
        public Transform BodyTransform;
        /// <summary>
        /// Layers to include for teleport check
        /// Remove the unnecessary ones to improve performance
        /// </summary>
        public LayerMask IncludeLayers;
        /// <summary>
        /// Teleporter ray arc angle
        /// </summary>
        public float Angle = 30f;
        /// <summary>
        /// Teleporter ray strength
        /// Increasing this value will increase the arc length
        /// </summary>
        public float Strength = 10f;
        /// <summary>
        /// Delta between each Vertex on arc.
        /// Decresing this value may cause performance problem.
        /// </summary>
        public float VertexDelta = 0.03f;
        #endregion // PUBLIC_MEMBERS
        //
        #region PRIVATE_MEMBERS
        // limitation of vertices for performance. 
        private int m_MaxVertexcount;
        // Delta between each Vertex on arc. Decresing this value may cause performance problem.
        //private float m_VertexDelta; 
        // Arc renderer
        private LineRenderer m_ArcRenderer;
        // Velocity of latest vertex
        private Vector3 m_Velocity;
        // detected ground position
        private Vector3 m_GroundPos;
        // detected surface normal
        private Vector3 m_LastNormal;
        // max number of ray points
        [SerializeField]
        private int m_MaxRayPoints;
        // Ground detection flag
        private bool m_GroundDetected = new bool();
        // don't update path when it's false.
        private bool m_DisplayActive = new bool();
        // vertex on arc
        private List<Vector3> m_VertexList = new List<Vector3>();
        // Dots of the arc
        private List<Transform> m_DotList = new List<Transform>();
        #endregion // PRIVATE_MEMBERS;
        //
        #region MONOBEHAVIOUR_METHODS
        // Awake
        private void Awake()
        {
            m_MaxVertexcount = 100;
            //m_VertexDelta = 0.08f;
            //
            m_ArcRenderer = GetComponent<LineRenderer>();
            //
            m_ArcRenderer.enabled = false;
            PositionMarker.SetActive(false);
            //
            CharacterController player = GetComponentInParent<CharacterController>();
            if (!BodyTransform && player != null)
            {
                BodyTransform = player.transform;
            }
            //
        }

        // Start is called before the first frame update
        void Start()
        {
            for (int i = 0; i < m_MaxRayPoints; i++)
            {
                GameObject ball = Instantiate(Dot, transform);

                ball.SetActive(false);
                m_DotList.Add(ball.transform);
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        // Physics update
        private void FixedUpdate()
        {
            if (m_DisplayActive)
            {
                UpdatePath();
            }
        }
        #endregion // MONOBEHAVIOUR_METHODS
        //
        #region PRIVATE_METHODS
        private void UpdatePath()
        {
            m_GroundDetected = false;
            PositionMarker.SetActive(false);
            // delete all previouse vertices
            m_VertexList.Clear();
            // hide the ray point objects
            ToggleRayPointDisplay(false);

            m_Velocity = Quaternion.AngleAxis(-Angle, transform.right) * transform.forward * Strength;

            RaycastHit hit;

            // take off position
            Vector3 pos = transform.position; 

            m_VertexList.Add(pos);

            while (!m_GroundDetected && m_VertexList.Count < m_MaxVertexcount)
            {
                Vector3 newPos = pos + m_Velocity * VertexDelta
                    + 0.5f * Physics.gravity * VertexDelta * VertexDelta;

                m_Velocity += Physics.gravity * VertexDelta;
                // add new calculated vertex
                m_VertexList.Add(newPos); 

                // linecast between last vertex and current vertex
                //if (Physics.Linecast(pos, newPos, out hit, IncludeLayers))
                if (Physics.Linecast(pos, newPos, out hit, IncludeLayers))
                {
                    // ignore non-horizontal surfaces
                    if (hit.normal == Vector3.up)
                    {
                        m_GroundDetected = true;
                        m_GroundPos = hit.point;
                        m_LastNormal = hit.normal;

                        break;
                    }
                }
                // update current vertex as last vertex
                pos = newPos; 
            }


            PositionMarker.SetActive(m_GroundDetected);

            if (m_GroundDetected)
            {
                PositionMarker.SetActive(true);
                PositionMarker.transform.position = m_GroundPos + m_LastNormal * 0.1f;
                PositionMarker.transform.LookAt(m_GroundPos);
            }

            // display ray points
            int limit = m_VertexList.Count - 1; 

            //limit = limit > 10 ? limit - 2 : limit; //ignore the last few points since they go beyond the landing 

            for (int i = 0; i < limit; i++)
            {
                m_DotList[i].position = m_VertexList[i];
                m_DotList[i].gameObject.SetActive(true);
            }

            // Update Line Renderer
            m_ArcRenderer.positionCount = m_VertexList.Count - 2;
            m_ArcRenderer.SetPositions(m_VertexList.ToArray());
        }


        private void ToggleRayPointDisplay(bool flag)
        {
            foreach (var item in m_DotList)
            {
                item.gameObject.SetActive(flag);
            }
        }
        #endregion // PRIVATE_METHODS
        //
        #region PUBLIC_METHODS
        /// <summary>
        /// Teleport the target object to the displayed ground position
        /// </summary>
        public void Teleport()
        {
            if (m_GroundDetected)
            {
                BodyTransform.localPosition = m_GroundPos + m_LastNormal * 0.1f;
            }
            else
            {
                Debug.Log("Ground wasn't detected");
            }
        }

        /// <summary>
        /// Show-Hide teleporter ray display
        /// True: Show
        /// False: Hide
        /// </summary>
        /// <param name="active"></param>
        public void ToggleDisplay(bool active)
        {
            gameObject.SetActive(active);
            m_ArcRenderer.enabled = active;
            PositionMarker.SetActive(active);
            m_DisplayActive = active;
        }
        #endregion //PUBLIC_METHODS
    }
}

