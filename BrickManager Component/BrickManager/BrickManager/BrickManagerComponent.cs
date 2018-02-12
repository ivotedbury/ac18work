using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Grasshopper.Kernel;
using Rhino.Geometry;

// In order to load the result of this wizard, you will also need to
// add the output bin/ folder of this project to the list of loaded
// folder in Grasshopper.
// You can use the _GrasshopperDeveloperSettings Rhino command for that.

namespace BrickManager
{
    public class BrickManagerComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public BrickManagerComponent()
          : base("BrickManager", "BrickM",
              "Converts Brick Arrangments into Objects and exports as JSon File.",
              "Brick Manager", "Main")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            // Use the pManager object to register your input parameters.
            // You can often supply default values when creating parameters.
            // All parameters must have the correct access type. If you want 
            // to import lists or trees of values, modify the ParamAccess flag.
            pManager.AddPointParameter("Origin", "O", "Origin for Brick Aggregation", GH_ParamAccess.item, Point3d.Origin);
            pManager.AddPointParameter("Brick origins", "BO", "Origin of each brick", GH_ParamAccess.item, Point3d.Origin);
            pManager.AddPointParameter("Brick X", "BX", "X of each brick", GH_ParamAccess.item, new Point3d(0.05125, 0, 0));
            pManager.AddPointParameter("Brick Y", "BY", "Y of each brick", GH_ParamAccess.item, new Point3d(0, 0.05625, 0));

            pManager.AddNumberParameter("Grid X", "gX", "Grid X dimension", GH_ParamAccess.item, 0.05625);
            pManager.AddNumberParameter("Grid Y", "gY", "Grid Y dimension", GH_ParamAccess.item, 0.05625);
            pManager.AddNumberParameter("Grid Z", "gZ", "Grid Z dimension", GH_ParamAccess.item, 0.0725);

            pManager.AddBooleanParameter("Calculate", "C", "Run the conversion", GH_ParamAccess.item, false);



            // If you want to change properties of certain parameters, 
            // you can use the pManager instance to access them by index:
            //pManager[0].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            // Use the pManager object to register your output parameters.
            // Output parameters do not have default values, but they too must have the correct access type.
            pManager.AddTextParameter("JSON", "J", "Output JSON file", GH_ParamAccess.item);

            // Sometimes you want to hide a specific parameter from the Rhino preview.
            // You can use the HideParameter() method as a quick way:
            //pManager.HideParameter(0);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // First, we need to retrieve all data from the input parameters.
            // We'll start by declaring variables and assigning them starting values.
            Point3d origin = Point3d.Origin;
            Point3d brickOrigins = Point3d.Origin;
            Point3d brickX = Point3d.Origin;
            Point3d brickY = Point3d.Origin;

            double gridDimX = 0.05625;
            double gridDimY = 0.05625;
            double gridDimZ = 0.0725;

            Boolean calculate = false;


            // Then we need to access the input parameters individually. 
            // When data cannot be extracted from a parameter, we should abort this method.
            if (!DA.GetData(0, ref origin)) return;
            if (!DA.GetData(1, ref brickOrigins)) return;
            if (!DA.GetData(2, ref brickX)) return;
            if (!DA.GetData(3, ref brickY)) return;

            if (!DA.GetData(4, ref gridDimX)) return;
            if (!DA.GetData(5, ref gridDimY)) return;
            if (!DA.GetData(6, ref gridDimZ)) return;

            if (!DA.GetData(7, ref calculate)) return;

            // We should now validate the data and warn the user if invalid data is supplied.
            if (gridDimX <= 0.0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Grid X dimension must be bigger than zero");
                return;
            }

            if (gridDimY <= 0.0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Grid Y dimension must be bigger than zero");
                return;
            }

            if (gridDimZ <= 0.0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Grid Z dimension must be bigger than zero");
                return;
            }


            // We're set to create the spiral now. To keep the size of the SolveInstance() method small, 
            // The actual functionality will be in a different method:
            String outputData = SerializeBrickArrangement(origin, brickOrigins, brickX, brickY, gridDimX, gridDimY, gridDimZ, calculate);

            // Finally assign the spiral to the output parameter.
            DA.SetData(0, outputData);
        }

        private string SerializeBrickArrangement(Point3d _origin, Point3d _brickOrigins, Point3d _brickX, Point3d _brickY, double _gridDimX, double _gridDimY, double _gridDimZ, Boolean _calculate)
        {
            string outputString = null;
           Brick brick = null;
            if (_calculate == true)
            {
                brick = GenerateBrick(_origin, _brickOrigins, _brickX, _brickY, _gridDimX, _gridDimY, _gridDimZ);
            }
            outputString = JsonConvert.SerializeObject(brick);

            return outputString;
        }

        private string CreateString(Brick _brick)
        {
            string list = Math.Round(_brick.gridPosition.X).ToString() + ", " + _brick.rotation.ToString();

            return list;
        }

        private class Brick
        {
            public Vector3d gridPosition;
            public double rotation;
            string brickType;

            public Brick(Vector3d _gridPos, double _rotation)
            {
                gridPosition = _gridPos;
                rotation = _rotation;
                brickType = "Standard";
            }
        }

        private Brick GenerateBrick(Point3d _origin, Point3d _brickOrigins, Point3d _brickX, Point3d _brickY, double _gridDimX, double _gridDimY, double _gridDimZ)
        {
            double posX;
            double posY;
            double posZ;
            Vector3d brickPosition;
            double rotationAngle = 0;

            Plane brickPlane = new Plane(_brickOrigins, _brickX, _brickY);

            posX = (_brickOrigins.X - _origin.X) / _gridDimX;
            posY = (_brickOrigins.Y - _origin.Y) / _gridDimY;
            posZ = (_brickOrigins.Z - _origin.Z) / _gridDimZ;

            brickPosition = new Vector3d(posX, posY, posZ);

            rotationAngle = Math.Round(Vector3d.VectorAngle(brickPlane.XAxis, Plane.WorldXY.XAxis) * 180 / Math.PI);

            //Vector3d xDirection = new Vector3d(_brickX - _brickOrigins);
            Vector3d yDirection = new Vector3d(_brickY - _brickOrigins);

           



            Brick outputBrick = new Brick(brickPosition, rotationAngle);


              return outputBrick;
        }

        private Curve CreateSpiral(Plane plane, double r0, double r1, Int32 turns)
        {
            Line l0 = new Line(plane.Origin + r0 * plane.XAxis, plane.Origin + r1 * plane.XAxis);
            Line l1 = new Line(plane.Origin - r0 * plane.XAxis, plane.Origin - r1 * plane.XAxis);

            Point3d[] p0;
            Point3d[] p1;

            l0.ToNurbsCurve().DivideByCount(turns, true, out p0);
            l1.ToNurbsCurve().DivideByCount(turns, true, out p1);

            PolyCurve spiral = new PolyCurve();

            for (int i = 0; i < p0.Length - 1; i++)
            {
                Arc arc0 = new Arc(p0[i], plane.YAxis, p1[i + 1]);
                Arc arc1 = new Arc(p1[i + 1], -plane.YAxis, p0[i + 1]);

                spiral.Append(arc0);
                spiral.Append(arc1);
            }

            return spiral;
        }

        /// <summary>
        /// The Exposure property controls where in the panel a component icon 
        /// will appear. There are seven possible locations (primary to septenary), 
        /// each of which can be combined with the GH_Exposure.obscure flag, which 
        /// ensures the component will only be visible on panel dropdowns.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.primary; }
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("7da25f25-0379-4afa-91cb-b63774b15d6b"); }
        }
    }
}
