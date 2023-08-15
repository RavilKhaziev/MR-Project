// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

HttpClient client = new HttpClient();

var a = await client.GetAsync("http://45.143.94.139/Discount/Products?Page=0");

Console.WriteLine(await a.Content.ReadAsStringAsync());