﻿using System;
using System.Linq;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Rhino.Geometry;
using Robots;
using Robots.Grasshopper;

namespace Extensions.View
{
    public class SpatialExtrusion : GH_Component
    {
        public SpatialExtrusion() : base("Spatial extrusion", "Spatial", "Create a toolpath spatial extrusion given a polyline. Requires the Robots plugin.", "Extensions", "Toolpaths")
        { }

        protected override System.Drawing.Bitmap Icon => Properties.Resources.Spatial;
        public override Guid ComponentGuid => new Guid("{79EE65B3-CB2F-4704-8B01-C7C9F379B7C4}");

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Polyline", "P", "Polyline", GH_ParamAccess.item);
            pManager.AddNumberParameter("Variables", "V", "0. Extrusion diameter (mm).\r\n1. Plunge distance (mm).\r\n2. Unsupported nodes vertical offset (mm).\r\n3. Unsupported segments rotation compensation (rad). \r\n4. Distance ahead to stop in upwards segments as a factor of length (0..1).\r\n5. Horizontal displacement before downward segment (mm).", GH_ParamAccess.list);
            pManager.AddParameter(new TargetParameter(), "Target", "T", "Target reference. Will use tool, frame and orientation of this target.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Speeds", "S", "0. Approach speed.\r\n1. Plunge speed.\r\n2. Supported segments\r\n3. Downward segments\r\n4. Unsupported nodes", GH_ParamAccess.list);
            pManager.AddNumberParameter("Wait times", "W", "0. Wait time after extrusion.\r\n1. Wait time ahead stop.\r\n2. Wait on supported node before unsupported segment.\r\n3. Wait on unsupported node.", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Digital outputs", "D", "The indices of the two digital outputs connected to the extruder.", GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new TargetParameter(), "Targets", "T", "Targets", GH_ParamAccess.list);
            pManager.AddLineParameter("Segments", "S", "Line segments", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Attributes", "A", "Segment types.\r\n0. Unsupported segment & unsupported node & up.\r\n1. Supported segment & unsupported node & up.\r\n2. Unsupported segment & supported node & up.\r\n3. Supported segment & Supported node & up.\r\n4. Unsupported segment & unsupported node & down.\r\n5. Supported segment & unsupported node & down.\r\n6. Unsupported segment & supported node & down.\r\n7. Supported segment & supported node & down.", GH_ParamAccess.list);
        }


        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Curve curve = null;
            if (!DA.GetData(0, ref curve)) return;
            if (!curve.TryGetPolyline(out var pl)) throw new Exception(" Curve must be a polyline.");

            var variables = new List<double>();
            if (!DA.GetDataList(1, variables)) return;

            GH_Target target = null;
            if (!DA.GetData(2, ref target)) return;
            if (!(target.Value is CartesianTarget)) throw new Exception(" Target must be a cartesian target.");

            var speeds = new List<double>();
            if (!DA.GetDataList(3, speeds)) return;

            var waits = new List<double>();
            if (!DA.GetDataList(4, waits)) return;

            var dos = new List<int>();
            if (!DA.GetDataList(5, dos)) return;

            var attributes = new Toolpaths.SpatialAttributes(variables, (target.Value) as CartesianTarget, speeds, waits, dos);
            var spatial = new Toolpaths.SpatialExtrusion(pl, attributes);

            DA.SetDataList(0, spatial.Targets);
            DA.SetDataList(1, spatial.Display.Select(d => d.segment));
            DA.SetDataList(2, spatial.Display.Select(d => d.type));
        }
    }
}