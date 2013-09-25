using System.Collections.Generic;

namespace GenericWindowsService.BL
{
    public interface IServiceItemsLoader
    {
        List<IGenericServiceItem> LoadAll();
        string ConfigFile { get; set; }
    }
}