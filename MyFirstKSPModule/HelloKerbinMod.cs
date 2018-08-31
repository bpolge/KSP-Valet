using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace MyFirstKSPModule
{
    public class HelloKerbinMod : PartModule
    {
        TargetingRenderer targetLines;
        public override void OnStart(StartState state)
        {
            Debug.Log("MY_FIRST: OnStart");

            // Make sure there aren't duplicates attached to the vessel
            vessel.OnFlyByWire -= OnFlyByWire;
            vessel.OnFlyByWire += OnFlyByWire;

            targetLines = FlightCamera.fetch.mainCamera.gameObject.AddComponent<TargetingRenderer>();
        }

        private ITargetable targetedVessel;
        private Vessel activeVessel;
        private Vector3 relVelocityTarget = new Vector3();
        private void Update()
        {
            targetLines.enabled = false;
            targetLines.Lines.Clear();
            if (HighLogic.LoadedSceneIsFlight && !FlightGlobals.ActiveVessel.isEVA && !MapView.MapIsEnabled)
            {
                targetedVessel = FlightGlobals.fetch.VesselTarget;
                activeVessel = FlightGlobals.ActiveVessel;

                if (targetedVessel == null) return;

                relVelocityTarget = targetedVessel.GetObtVelocity() - activeVessel.GetObtVelocity();

                //FlightGlobals.ActiveVessel.ActionGroups.ToggleGroup(KSPActionGroup.RCS);
                if (relVelocityTarget != null && !relVelocityTarget.IsZero())
                {
                    Vector3 v = Quaternion.Inverse(vessel.GetTransform().rotation) * relVelocityTarget;
                    Vector3 p = activeVessel.transform.position;
                    Vector3 tP = targetedVessel.GetTransform().position;

                    TargetingLine vesselToTarget = new TargetingLine(p, tP);
                    targetLines.Add(vesselToTarget);

                    //st.X = clamp((float)v.x, -1, 1);
                    //st.Y = clamp((float)v.z, -1, 1);
                    //st.Z = clamp((float)v.y, -1, 1);
                }
                targetLines.enabled = true;


            }
        }

        private void OnFlyByWire(FlightCtrlState st)
        {

        }

        private float clamp(float x, float min, float max)
        {
            if (x < min) return min;
            if (x > max) return max;
            return x;
        }
        

    }

    public class TargetingLine {
        public List<Vector3> Verticies = new List<Vector3>();

        public Vector3 From {
            get => Verticies[0];
            set => Verticies.Insert(0, value);
        }

        public Vector3 To {
            get => Verticies[1];
            set => Verticies.Insert(1, value);
        }

        public TargetingLine(Vector3 from, Vector3 to)
        {
            this.From = from;
            this.To = to;
        }
    }

    public class TargetingRenderer: MonoBehaviour
    {
        public List<TargetingLine> Lines = new List<TargetingLine>();

        static float lineWidth = 2;
        public float LineWidth {
            set => lineWidth = value;
        }
        private Material material;

        public void Add(TargetingLine line) {
            Lines.Add(line);
        }

        public void OnPostRender()
        {
            if (Lines.Count == 0) return;
            //Debug.Log("DRAWING!!!");
            if (material == null) material = new Material(Shader.Find("Particles/Additive"));

            foreach (TargetingLine line in Lines)
            {
                GL.PushMatrix();
                material.SetPass(0);
                GL.LoadPixelMatrix();
                GL.Begin(GL.QUADS);
                GL.Color(Color.blue);

                Debug.Log("ADDING POINTS");
                Debug.Log(this.enabled);
                //for (int i = 0; i < Vertices.Count; i++)
                //{
                //    Vector3 p = Vertices[i];
                //    Vector3 sp = FlightCamera.fetch.mainCamera.WorldToScreenPoint(p);
                //    Debug.Log(sp);
                //    GL.Vertex3(p.x, p.z, 0);
                //}
                foreach (Vector3 vertex in line.Verticies)
                {
                    Vector3 point = FlightCamera.fetch.mainCamera.WorldToScreenPoint(vertex);
                    GL.Vertex3(point.x, point.y, 0);
                    GL.Vertex3(point.x + lineWidth, point.y, 0);
                }
                GL.End();
                GL.PopMatrix();
            }
        }
    }

    //public override void Drive(FlightCtrlState s)
    //{
    //    setPIDParameters();

    //    switch (controlType)
    //    {
    //        case ControlType.TARGET_VELOCITY:
    //            // Removed the gravity since it also affect the target and we don't know the target pos here.
    //            // Since the difference is negligable for docking it's removed
    //            // TODO : add it back once we use the RCS Controler for other use than docking. Account for current acceleration beside gravity ?
    //            worldVelocityDelta = vesselState.orbitalVelocity - targetVelocity;
    //            //worldVelocityDelta += TimeWarp.fixedDeltaTime * vesselState.gravityForce; //account for one frame's worth of gravity
    //            //worldVelocityDelta -= TimeWarp.fixedDeltaTime * gravityForce = FlightGlobals.getGeeForceAtPosition(  Here be the target position  ); ; //account for one frame's worth of gravity
    //            break;

    //        case ControlType.VELOCITY_ERROR:
    //            // worldVelocityDelta already contains the velocity error
    //            break;

    //        case ControlType.VELOCITY_TARGET_REL:
    //            if (core.target.Target == null)
    //            {
    //                rcsDeactivate();
    //                return;
    //            }

    //            worldVelocityDelta = core.target.RelativeVelocity - targetVelocity;
    //            break;
    //    }

    //    // We work in local vessel coordinate
    //    Vector3d velocityDelta = Quaternion.Inverse(vessel.GetTransform().rotation) * worldVelocityDelta;

    //    if (!conserveFuel || (velocityDelta.magnitude > conserveThreshold))
    //    {
    //        if (!vessel.ActionGroups[KSPActionGroup.RCS])
    //        {
    //            vessel.ActionGroups.SetGroup(KSPActionGroup.RCS, true);
    //        }

    //        Vector3d rcs = new Vector3d();

    //        for (int i = 0; i < Vector6.Values.Length; i++)
    //        {
    //            Vector6.Direction dir = Vector6.Values[i];
    //            double dirDv = Vector3d.Dot(velocityDelta, Vector6.directions[(int)dir]);
    //            double dirAvail = vesselState.rcsThrustAvailable[dir];
    //            if (dirAvail > 0 && Math.Abs(dirDv) > 0.001)
    //            {
    //                double dirAction = dirDv / (dirAvail * TimeWarp.fixedDeltaTime / vesselState.mass);
    //                if (dirAction > 0)
    //                {
    //                    rcs += Vector6.directions[(int)dir] * dirAction;
    //                }
    //            }
    //        }

    //        Vector3d omega = Vector3d.zero;

    //        switch (controlType)
    //        {
    //            case ControlType.TARGET_VELOCITY:
    //                omega = Quaternion.Inverse(vessel.GetTransform().rotation) * (vessel.acceleration - vesselState.gravityForce);
    //                break;

    //            case ControlType.VELOCITY_TARGET_REL:
    //            case ControlType.VELOCITY_ERROR:
    //                omega = (worldVelocityDelta - prev_worldVelocityDelta) / TimeWarp.fixedDeltaTime;
    //                prev_worldVelocityDelta = worldVelocityDelta;
    //                break;
    //        }

    //        rcs = pid.Compute(rcs, omega);

    //        // Disabled the low pass filter for now. Was doing more harm than good
    //        //rcs = lastAct + (rcs - lastAct) * (1 / ((Tf / TimeWarp.fixedDeltaTime) + 1));
    //        lastAct = rcs;

    //        s.X = Mathf.Clamp((float)rcs.x, -1, 1);
    //        s.Y = Mathf.Clamp((float)rcs.z, -1, 1); //note that z and
    //        s.Z = Mathf.Clamp((float)rcs.y, -1, 1); //y must be swapped
    //    }
    //    else if (conserveFuel)
    //    {
    //        if (vessel.ActionGroups[KSPActionGroup.RCS])
    //        {
    //            vessel.ActionGroups.SetGroup(KSPActionGroup.RCS, false);
    //        }
    //    }

    //    base.Drive(s);
    //}
}
