using System.Collections.Generic;

//#if UNITY_EDITOR
namespace catalogcreator
{
    public class Root
    {
        public string appleSKU { get; set; }
        public string appleTeamID { get; set; }
        public bool enableCodelessAutoInitialization { get; set; }
        public List<Product> products { get; set; }
    }


    public class DefaultDescription
    {
        public int googleLocale { get; set; }
        public string title { get; set; }
        public string description { get; set; }
    }

    public class GooglePrice
    {
        public List<int> data { get; set; }
        public double num { get; set; }
    }

    public class Product
    {
        public string id { get; set; }
        public int type { get; set; }
        public List<storeID> storeIDs { get; set; }
        public DefaultDescription defaultDescription { get; set; }
        public string screenshotPath { get; set; }
        public int applePriceTier { get; set; }
        public GooglePrice googlePrice { get; set; }
        public string pricingTemplateID { get; set; }
        public List<object> descriptions { get; set; }
        public UdpPrice udpPrice { get; set; }
        public List<object> payouts { get; set; }
    }

    public class storeID
    {
        public string store = "",
    id = "";
    }

    public class UdpPrice
    {
        public List<int> data { get; set; }
        public double num { get; set; }
    }

    //#endif

}
