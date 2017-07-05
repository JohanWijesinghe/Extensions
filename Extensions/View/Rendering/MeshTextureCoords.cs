﻿using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Rhino.Geometry;
using Extensions;


namespace Extensions.View
{
    public class MeshTextureCoords : GH_Component
    {
        public MeshTextureCoords() : base("Mesh texture coordinates", "MeshTexCoords", "Sets mesh texture coordinates.", "Extensions", "Rendering") { }
        protected override System.Drawing.Bitmap Icon => Properties.Resources.Cube;
        public override Guid ComponentGuid => new Guid("{297d173d-4eac-4a93-947c-fa8216e73cfa}");

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "M", "Single mesh with render colors.", GH_ParamAccess.item);
            pManager.AddPointParameter("Texture coords", "T", "Texture coordinates. Must equal the number of mesh vertices. The Z components will be ignored.", GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "M", "Mesh with texture coordinates set.", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Mesh mesh = new Mesh();
            var coords = new List<Point3d>();
            DA.GetData(0, ref mesh);
            DA.GetDataList(1, coords);

            Mesh outMesh = RenderExtensions.SetTextureCoords(mesh, coords);
            DA.SetData(0, outMesh);
        }
    }
}