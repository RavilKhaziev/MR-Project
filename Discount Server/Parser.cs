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
        /// <summary> 
        /// Функция запрашивает у парсера, все доступные категории товаров для парсера.
        /// </summary>
        /// <returns>Лист названий категорий.</returns>
        public List<string> GetProductsCategory()
        {
            throw new NotImplementedException();
        }

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
                {"myaso_ptitsa_kolbasy", 1 },
                {"khleb_vypechka_sneki", 0 },
                {"ovoshchi_frukty", 2 },

            };

            //Console.WriteLine("1.Мясо\n2.Хлеб\n3.Овощи и фрукты");
            //int typeInd = Int32.Parse(Console.ReadLine());
            //type = typeProducts[typeInd - 1];

            foreach (var element in types_magnit)
            {
                var request = new GetRequest($"https://dostavka.magnit.ru/express/catalog/{element.Key}?hasDiscount=true&page=1");
                request.Run();
                List<int> list_code = new List<int>();
                string response = request.Response;

                int index = 0;
                int searchStartName, searchEndName, searchStartNewPrice, searchEndNewPrice, searchStartOldPrice, searchEndOldPrice;
                int searchStartProductCard, searchEndProductCard;
                string nameProduct, NewPriceProduct, OldPriceProduct, product_card;
                List<string> names = new();
                List<long> prices = new();
                List<string> types_prod = new();
                List<string> descs = new();
                List<string> img_url = new();
                List<string> url_products = new();

                while (response.IndexOf("\"product-item-title\" title=\"", index) != -1)
                {
                    searchStartProductCard = response.IndexOf("id=\"product-card-", index) + 17;
                    searchEndProductCard = response.IndexOf("data", searchStartProductCard) - 2;

                    product_card = response.Substring(searchStartProductCard, searchEndProductCard - searchStartProductCard);

                    list_code.Add(Int32.Parse(product_card));

                    index = searchEndProductCard;

                    searchStartName = response.IndexOf("\"product-item-title\" title=\"", index) + 28;
                    searchEndName = response.IndexOf("\" class=\"text\"", searchStartName);

                    nameProduct = response.Substring(searchStartName, searchEndName - searchStartName);

                    names.Add(nameProduct);

                    index = searchEndName;

                    searchStartNewPrice = response.IndexOf("\"m-price__current is-discounted\"><span>", index) + 39;
                    searchEndNewPrice = response.IndexOf("</span>", searchStartNewPrice);

                    NewPriceProduct = response.Substring(searchStartNewPrice, searchEndNewPrice - searchStartNewPrice);

                    prices.Add(long.Parse(NewPriceProduct));

                    index = searchEndNewPrice;

                    searchStartOldPrice = response.IndexOf("\"m-price__old\"><span>", index) + 21;
                    searchEndOldPrice = response.IndexOf("</span>", searchStartOldPrice) - 2;

                    OldPriceProduct = response.Substring(searchStartOldPrice, searchEndOldPrice - searchStartOldPrice);

                    index = searchEndOldPrice;

                    types_prod.Add(types[element.Value]);

                    Console.WriteLine("\n////////////\n");
                    Console.WriteLine($"Название: {nameProduct}");
                    Console.WriteLine($"Старая цена: {OldPriceProduct} Р.");
                    Console.WriteLine($"Новая цена: {NewPriceProduct} Р.");

                    //products.Add(new ProductInfoModel { Name = nameProduct, Sale_Price = long.Parse(NewPriceProduct), Description = " ", Image_Url = " ", Url = " ", Type = types[element.Value] });

                }

                int searchId, searchStartCode, searchEndCode;
                string code;
                List<string> url_card = new List<string>();
                foreach (var item in list_code)
                {
                    searchId = response.IndexOf($"id:{item}", index);

                    searchStartCode = response.IndexOf($"code:\"", searchId) + 6;
                    searchEndCode = response.IndexOf($",article", searchId) - 1;

                    code = response.Substring(searchStartCode, searchEndCode - searchStartCode);
                    url_card.Add(code);
                }
                index = 0;
                string desc, url_image, url_product;
                int searchEndJpeg, searchStartJpeg;
                int searchStartdesc, searchEnddesc;
                foreach (var item in url_card)
                {
                    searchEndJpeg = 0;
                    searchStartJpeg = 0;
                    index = 0;
                    url_product = $"https://dostavka.magnit.ru/express/product/{item}";
                    url_products.Add(url_product);

                    request = new GetRequest(url_product);
                    request.Run();
                    response = request.Response;

                    searchStartdesc = response.IndexOf("product-detail-text\">", index) + 20;
                    searchEnddesc = response.IndexOf("</div>", searchStartdesc) - 1;

                    desc = response.Substring(searchStartdesc, searchEnddesc - searchStartdesc);

                    descs.Add(desc);

                    while ((searchEndJpeg - searchStartJpeg) < 30)
                    {
                        searchEndJpeg = response.IndexOf(".jpeg", index) + 4;
                        if ((searchEndJpeg - 4) == -1) searchEndJpeg = response.IndexOf(".png", index) + 3;
                        searchStartJpeg = searchEndJpeg;

                        while (response[searchStartJpeg] != '\"')
                        {
                            searchStartJpeg--;
                        }
                        searchStartJpeg++;
                        index = searchEndJpeg;
                    }


                    url_image = "https://img-dostavka.magnit.ru/resize/420x420/" + response.Substring(searchStartJpeg, searchEndJpeg - searchStartJpeg) + "g";
                    url_image = url_image.Replace("\\u002F", "/");

                    img_url.Add(url_image);
                }

                for (int i = 0; i < names.Count; i++)
                {
                    products.Add(new ProductInfoModel { Name = names[i], Description = descs[i], Image_Url = img_url[i], Sale_Price = prices[i], Type = types_prod[i], Url = url_products[i] });
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
                string img_url;
                for (int i = 0; i < mass["results"].AsArray().Count; i++)
                {
                    Console.WriteLine("\n//////\n");
                    Console.WriteLine($"Имя: {mass["results"][i]["name"]}");
                    Console.WriteLine($"Старая цена: {mass["results"][i]["current_prices"]["price_reg__min"]}");
                    Console.WriteLine($"Новая цена: {mass["results"][i]["current_prices"]["price_promo__min"]}");
                    name = mass["results"][i]["name"].ToString();
                    var tmp = mass["results"][i]["current_prices"]["price_promo__min"];
                    img_url = mass["results"][i]["img_link"].ToString();
                    price = long.Parse(tmp.ToString().Substring(0, tmp.ToString().IndexOf('.')));
                    products.Add(new ProductInfoModel { Name = name, Sale_Price = price, Description = " ", Image_Url = img_url, Url = " ", Type = types[element.Value] });
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
