using System;

namespace GenericWindowsService.BL
{
    public class ServiceItemExecutableDefinition : IExecutableDefinition
    {
        public string DllName { get; set; }
        public string FullyQualifiedClassName { get; set; }
        public string MethodName { get; set; }
        public string Path { get; set; }

        public string GetFullPath()
        {
            if (string.IsNullOrEmpty(DllName))
            {
                throw new MissingFieldException("ServiceItemExecutableDefinition", "DllName");
            }
            else
            {
                if (string.IsNullOrEmpty(Path))
                {
                    Path = Environment.CurrentDirectory;
                }

                return System.IO.Path.Combine(Path, DllName);
            }
        }
    }
}