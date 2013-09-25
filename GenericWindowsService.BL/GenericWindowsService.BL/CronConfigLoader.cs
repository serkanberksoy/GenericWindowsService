using System.Collections.Generic;

namespace GenericWindowsService.BL
{
    public class CronConfigLoader : IServiceItemsLoader
    {
        public string ConfigFile { get; set; }

        public List<IGenericServiceItem> LoadAll()
        {
            throw new System.NotImplementedException();
        }

        public struct SServiceItemConfig
        {
        }
    }
}