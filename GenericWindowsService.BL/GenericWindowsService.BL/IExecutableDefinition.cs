namespace GenericWindowsService.BL
{
    public interface IExecutableDefinition
    {
        string ServiceName { get; set; }
        string DllName { get; set; }
        string FullyQualifiedClassName { get; set; }
        string MethodName { get; set; }
        string Path { get; set; }

        string GetFullPath();
    }
}