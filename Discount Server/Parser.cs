using Discount_Server.ViewModels;
using System.Net;
using System.Text.Json.Nodes;
using System.Text;
using Discount_Server.Models;

namespace Discount_Server
{
    interface IParser
    {
        /// <summary> 
        /// Функция запрашивает у парсера, все доступные магазины для парсера.
        /// </summary>
        /// <returns>Лист магазинов. Значение может быть null.</returns>
        public Task<List<ShopInfoModel>?> GetShopListAsync();
        /// <summary> 
        /// Функция запрашивает у парсера, продукты конкретного магазина.
        /// </summary>
        /// <param name="shop"> Лист продуктов будет из данного магазина </param>
        /// <returns>Лист продуктов из указанного магазина. Значение может быть null.</returns>
        public Task<List<ProductInfoModel>?> GetProductListAsync(ShopInfo shop);

        public List<ShopInfoModel> GetShopList();

        //return products
        public List<ProductInfoModel> GetProductList(ShopInfoModel shop);

    }

    //class Parser : IParser
    //{
    //    public Parser() { }

    //    class ShopModel
    //    {
    //        static public ShopInfoModel Magnit { get; } =
    //            new ShopInfoModel
    //            {
    //                Name = "Магнит",
    //                Shop_Code = 0,
    //                Url = "https://magnit.ru/"
    //            };
    //        static public ShopInfoModel Peterychka { get; } =
    //            new ShopInfoModel
    //            {
    //                Name = "Пятёрочка",
    //                Shop_Code = 1,
    //                Url = "https://5ka.ru/"
    //            };
    //    }

    //    class ProductModel
    //    {
    //        static public List<ProductInfoModel> Magnit { get; } = new List<ProductInfoModel>
    //        {
    //            new ProductInfoModel{ Name = "Хлеб вкусный", Description = "Очень вкусный хлеб. Пшеничный.", Image_Url = "Заглушка", Sale_Price = 50L, Type = "Хлеб", Url = "Заглушка" },
    //            new ProductInfoModel{ Name = "Яблоки соч. красн.", Description = "Сочные, красные яблоки.", Image_Url = "Заглушка", Sale_Price = 150L, Type = "Фрукты", Url = "Заглушка" },
    //            new ProductInfoModel{ Name = "Картошка Бел.", Description = "Картошка", Image_Url = "Заглушка", Sale_Price = 456L, Type = "Овощи", Url = "Заглушка" },

    //        };

    //        static public List<ProductInfoModel> Peterychka { get; } = new List<ProductInfoModel>
    //        {
    //            new ProductInfoModel{ Name = "Хлеб вкусный", Description = "Очень вкусный хлеб. Пшеничный.", Image_Url = "Заглушка", Sale_Price = 50L, Type = "Хлеб", Url = "Заглушка" },
    //            new ProductInfoModel{ Name = "Яблоки соч. красн.", Description = "Сочные, красные яблоки.", Image_Url = "Заглушка", Sale_Price = 150L, Type = "Фрукты", Url = "Заглушка" },
    //            new ProductInfoModel{ Name = "Картошка Бел.", Description = "Картошка", Image_Url = "Заглушка", Sale_Price = 456L, Type = "Овощи", Url = "Заглушка" },
    //        };
    //    }

    //    / <summary> 
    //    / Функция запрашивает у парсера, все доступные магазины для парсера.
    //    / </summary>
    //    / <returns>Лист магазинов. Значение может быть null.</returns>
    //    public async Task<List<ShopInfoModel>?> GetShopList()
    //    {
    //        return new List<ShopInfoModel> { ShopModel.Magnit, ShopModel.Peterychka };
    //    }

    //    / <summary> 
    //    / Функция запрашивает у парсера, продукты конкретного магазина.
    //    / </summary>
    //    / <param name = "shop" > Лист продуктов будет из данного магазина</param>
    //    / <returns>Лист продуктов из указанного магазина.Значение может быть null.</returns>
    //    public async Task<List<ProductInfoModel>?> GetProductList(ShopInfo shop)
    //    {
    //        if (ShopModel.Magnit.Name == shop.Shop_Name)
    //        {
    //            return ProductModel.Magnit;
    //        }
    //        if (ShopModel.Peterychka.Name == shop.Shop_Name)
    //        {
    //            return ProductModel.Peterychka;
    //        }
    //        return null;
    //    }
    //}


    class Parser : IParser
    {
        static List<ShopInfoModel> shops = new List<ShopInfoModel>()
        {
            new ShopInfoModel{Name = "Magnit", Shop_Code = 0, Url = "https://dostavka.magnit.ru/"},
            new ShopInfoModel{Name = "Pyatorochka", Shop_Code = 1, Url = "https://5ka.ru/" }
        };

        static Dictionary<int, string> types = new Dictionary<int, string>()
        {
            {0, "bread"},
            {1, "meat" },
            {2, "fruits and vegetables" }
        };

        public class GetRequest
        {
            HttpWebRequest _request;
            string _address;

            public string Response { get; set; }

            public string Accepct { get; set; }

            public string Host { get; set; }

            public string Refer { get; set; }

            public string Useragent { get; set; }

            public WebProxy Proxy { get; set; }

            public Dictionary<string, string> Headers { get; set; }

            public GetRequest(string address)
            {
                _address = address;
                Headers = new Dictionary<string, string>();
            }

            public void Run()
            {
                _request = (HttpWebRequest)WebRequest.Create(_address);
                _request.Method = "GET";

                try
                {
                    HttpWebResponse response = (HttpWebResponse)_request.GetResponse();
                    var stream = response.GetResponseStream();

                    if (stream != null) Response = new StreamReader(stream).ReadToEnd();
                }
                catch (Exception)
                {


                }

            }

            public void Run(CookieContainer cookieContainer)
            {
                _request = (HttpWebRequest)WebRequest.Create(_address);
                _request.Method = "GET";
                _request.CookieContainer = cookieContainer;
                _request.Proxy = Proxy;
                _request.Accept = Accepct;
                _request.Host = Host;
                _request.Referer = Refer;
                _request.UserAgent = Useragent;

                foreach (var pair in Headers)
                {
                    _request.Headers.Add(pair.Key, pair.Value);
                }
                try
                {
                    HttpWebResponse response = (HttpWebResponse)_request.GetResponse();
                    var stream = response.GetResponseStream();

                    if (stream != null) Response = new StreamReader(stream).ReadToEnd();
                }
                catch (Exception)
                {


                }
            }
        }

        public class PostRequest 
        {
            HttpWebRequest _request;
            string _address;

            public string Response { get; set; }

            public string Accepct { get; set; }

            public string Host { get; set; }

            public string Refer { get; set; }

            public string Useragent { get; set; }

            public string Data { get; set; }

            public string ContentType { get; set; }

            public WebProxy Proxy { get; set; }

            public Dictionary<string, string> Headers { get; set; }

            public PostRequest(string address)
            {
                _address = address;
                Headers = new Dictionary<string, string>();
            }

            public void Run()
            {
                _request = (HttpWebRequest)WebRequest.Create(_address);
                _request.Method = "POST";

                try
                {
                    HttpWebResponse response = (HttpWebResponse)_request.GetResponse();
                    var stream = response.GetResponseStream();

                    if (stream != null) Response = new StreamReader(stream).ReadToEnd();
                }
                catch (Exception)
                {


                }

            }

            public void Run(CookieContainer cookieContainer)
            {
                _request = (HttpWebRequest)WebRequest.Create(_address);
                _request.Method = "POST";
                _request.CookieContainer = cookieContainer;
                _request.Proxy = Proxy;
                _request.Accept = Accepct;
                _request.Host = Host;
                _request.Referer = Refer;
                _request.UserAgent = Useragent;

                byte[] sentData = Encoding.UTF8.GetBytes(Data);
                _request.ContentLength = sentData.Length;
                Stream sentStream = _request.GetRequestStream();
                sentStream.Write(sentData, 0, sentData.Length);
                sentStream.Close();


                foreach (var pair in Headers)
                {
                    _request.Headers.Add(pair.Key, pair.Value);
                }
                try
                {
                    HttpWebResponse response = (HttpWebResponse)_request.GetResponse();
                    var stream = response.GetResponseStream();

                    if (stream != null) Response = new StreamReader(stream).ReadToEnd();
                }
                catch (Exception)
                {


                }
            }
        }

        public static void ParseMagnit(ref List<ProductInfoModel> products)
        {
            //string[] typeProducts = new string[] { "myaso_ptitsa_kolbasy", "khleb_vypechka_sneki", "ovoshchi_frukty" };
            string type;

            Dictionary<string, int> types_magnit = new Dictionary<string, int>()
            {
                {"khleb_vypechka_sneki", 0 },
                {"myaso_ptitsa_kolbasy", 1 },
                {"ovoshchi_frukty", 2 },

            };

            //Console.WriteLine("1.Мясо\n2.Хлеб\n3.Овощи и фрукты");
            //int typeInd = Int32.Parse(Console.ReadLine());
            //type = typeProducts[typeInd - 1];

            foreach (var element in types_magnit)
            {
                var request = new GetRequest($"https://dostavka.magnit.ru/express/catalog/{element.Key}?hasDiscount=true&page=1");
                request.Run();

                string response = request.Response;

                int index = 0;
                int searchStartName, searchEndName, searchStartNewPrice, searchEndNewPrice, searchStartOldPrice, searchEndOldPrice;
                string nameProduct, NewPriceProduct, OldPriceProduct;
                
                // to do 
                // Если ответа нет будет вызывать ошибку 
                // Необходимо отловить ошибку или предложить другое решение 
                while (response.IndexOf("\"product-item-title\" title=\"", index) != -1) // To do
                {
                    searchStartName = response.IndexOf("\"product-item-title\" title=\"", index) + 28;
                    searchEndName = response.IndexOf("\" class=\"text\"", searchStartName);

                    nameProduct = response.Substring(searchStartName, searchEndName - searchStartName);

                    index = searchEndName;

                    searchStartNewPrice = response.IndexOf("\"m-price__current is-discounted\"><span>", index) + 39;
                    searchEndNewPrice = response.IndexOf("</span>", searchStartNewPrice);

                    NewPriceProduct = response.Substring(searchStartNewPrice, searchEndNewPrice - searchStartNewPrice);

                    index = searchEndNewPrice;

                    searchStartOldPrice = response.IndexOf("\"m-price__old\"><span>", index) + 21;
                    searchEndOldPrice = response.IndexOf("</span>", searchStartOldPrice) - 2;

                    OldPriceProduct = response.Substring(searchStartOldPrice, searchEndOldPrice - searchStartOldPrice);

                    index = searchEndOldPrice;

                    products.Add(new ProductInfoModel { Name = nameProduct, Sale_Price = long.Parse(NewPriceProduct), Description = " ", Image_Url = " ", Url = " ", Type = types[element.Value] });

                }
            }

        }

        public static void ParsePyat(ref List<ProductInfoModel> products)
        {
            //Хлеб 888. Фрукты ягоды орехи - 2253
            //int[] typeProducts = new int[] { 888, 2253 };
            //int type;

            Dictionary<int, int> types_pyat = new Dictionary<int, int>()
            {
                {888, 0 },
                {2253, 2 },

            };

            //Console.WriteLine("1.Хлеб\n2.Овощи и фрукты");
            //int typeInd = Int32.Parse(Console.ReadLine());
            //type = typeProducts[typeInd - 1];
            foreach (var element in types_pyat)
            {
                var request = new GetRequest($"https://5ka.ru/api/v2/special_offers/?records_per_page=15&page=1&store=31Z6&ordering=&price_promo__gte=&price_promo__lte=&categories={element.Key}&search=");
                //request.Accepct = "application/json, text/plain, */*";

                request.Run();

                string response = request.Response;
                //JsonSerializer json = JsonSerializer.Deserialize(response); 
                var mass = JsonObject.Parse(response);
                //Console.WriteLine(mass["results"].AsArray());
                //Console.WriteLine(mass["results"].AsArray()[0]["name"]);
                string name;
                long price;
                for (int i = 0; i < mass["results"].AsArray().Count; i++)
                {
                    name = mass["results"][i]["name"].ToString();
                    var tmp = mass["results"][i]["current_prices"]["price_promo__min"]; ;
                    price = long.Parse(tmp.ToString().Substring(0, tmp.ToString().IndexOf('.')));
                    products.Add(new ProductInfoModel { Name = name, Sale_Price = price, Description = " ", Image_Url = " ", Url = " ", Type = types[element.Value] });
                }
            }

        }

        //return shops
        public List<ShopInfoModel> GetShopList()
        {
            return shops;
        }

        //return products
        public List<ProductInfoModel> GetProductList(ShopInfoModel shop)
        {
            List<ProductInfoModel> products = new List<ProductInfoModel>();

            switch (shop.Name)
            {
                case "Magnit":
                    ParseMagnit(ref products);
                    break;
                case "Pyatorochka":
                    ParsePyat(ref products);
                    break;
                default:
                    break;
            }

            return products;
        }

        public Task<List<ShopInfoModel>?> GetShopListAsync()
        {
            throw new NotImplementedException();
        }

        public Task<List<ProductInfoModel>?> GetProductListAsync(ShopInfo shop)
        {
            throw new NotImplementedException();
        }

    }
}
