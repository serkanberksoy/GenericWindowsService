namespace GenericWindowsService.BL
{
    public interface IExecutableDefinition
    {
        string DllName { get; set; }
        string FullyQualifiedClassName { get; set; }
        string MethodName { get; set; }
        string Path { get; set; }
        string GetFullPath();
    }
}