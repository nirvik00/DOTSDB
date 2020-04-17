using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace DotsDBProj
{
    public class DotsDBProjInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "DotsDBProj";
            }
        }
        public override Bitmap Icon
        {
            get
            {
                //Return a 24x24 pixel bitmap to represent this GHA library.
                return null;
            }
        }
        public override string Description
        {
            get
            {
                //Return a short string describing the purpose of this GHA library.
                return "";
            }
        }
        public override Guid Id
        {
            get
            {
                return new Guid("e361c95e-a5c4-4261-88e4-7902c3c573fa");
            }
        }

        public override string AuthorName
        {
            get
            {
                //Return a string identifying you or your company.
                return "";
            }
        }
        public override string AuthorContact
        {
            get
            {
                //Return a string representing your preferred contact details.
                return "";
            }
        }
    }
}
