using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;



namespace WebscraperLK
{
    class Program
    {
        static void Main(string[] args)
        {
            bool showMenu = true;
            while (showMenu)
            {
                showMenu = MainMenu();
            }
        }


        private static bool MainMenu()
        {
            

            Console.Clear();
            Console.WriteLine("What would you like to choose:");
            Console.WriteLine("1) Search for videos on Youtube");
            Console.WriteLine("2) Search for jobs on a jobsite");
            Console.WriteLine("3) Exit");
            Console.Write("\nSelect an option:");

            switch (Console.ReadLine())
            {
                case "1":
                    Console.WriteLine("Enter a searchterm for a video: ");
                    var searchterm = Console.ReadLine();
                    Youtube(searchterm);
                    return true;
                case "2":
                    Console.WriteLine("Enter a searchterm for a job: ");
                    var jobsearch = Console.ReadLine();
                    JobSite(jobsearch);
                    return true; 
                case "3":
                    return false;
                default:
                    return true;
            }




        }



        public static void addRecordToCSV(string value1, string value2, string value3, string value4, string value5, string filepath)
        {
            
            
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@filepath, true))
                {
                    file.WriteLine(value1 + ";" + value2 + ";" + value3 + ";" + value4 + ";" + value5);
                }
            
            

        }

        public static void addToJson(string id, string data, Boolean startObject, Boolean endObject)
        {
            TextWriter twJson = new StreamWriter("dataBoth.json");
            string jsonBothData = "[\n";

            if (startObject)
            {
                jsonBothData += "{\n\"" + id + "\": \"" + data + "\",\n";
            }
            else if (endObject)
            {
                jsonBothData += "\"" + id + "\": \"" + data + "\"\n },";
            }
            else
            {
                jsonBothData += "\"" + id + "\": \"" + data + "\",\n";
            }

            jsonBothData = jsonBothData.Substring(0, jsonBothData.Length - 1);
            jsonBothData += "\n]";
            twJson.Write(jsonBothData);
            twJson.Close();
            twJson.Close();
        }




        static void Youtube(string searchTerm)
        {
            IWebDriver driver = new ChromeDriver();

            driver.Navigate().GoToUrl("https://www.youtube.com/results?search_query=" + searchTerm + "&sp=CAI%253D");

            var cookiesMessage = driver.FindElement(By.XPath("//*[@id=\"content\"]/div[2]/div[6]/div[1]/ytd-button-renderer[1]/yt-button-shape/button/yt-touch-feedback-shape/div/div[2]"));
            cookiesMessage.Click();

            var titles = driver.FindElements(By.XPath("//*[@id=\"video-title\"]/yt-formatted-string"));
            var views = driver.FindElements(By.XPath("//*[@id=\"metadata-line\"]/span[1]"));
            var releases = driver.FindElements(By.XPath("//*[@id=\"metadata-line\"]/span[2]"));
            var uploaders = driver.FindElements(By.XPath("//*[@id=\"channel-info\"]"));
            var links = driver.FindElements(By.XPath("//*[@id=\"video-title\"]"));

            int count = 0;

            while (count < 5)
            {
                Console.WriteLine("");
                string title = titles[count].Text;
                string view = views[count].Text;
                string release = releases[count].Text;
                string uploader = uploaders[count].Text;
                string link = links[count].GetAttribute("href");
                Console.WriteLine("Title: " + title);
                Console.WriteLine("Amount of views: " + view);
                Console.WriteLine("Released on: " + release);
                Console.WriteLine("Uploaded by: " + uploader);
                Console.WriteLine("Link of the video: " + link);
                addToJson("Link", link, true, false);
                addToJson("Title", title.Replace("\"", ""), false, false);
                addToJson("Uploader", uploader, false, false);
                addToJson("Views", view, false, true);

                addRecordToCSV(title, view, release, uploader, link, "video.csv");
                count++;
            }


            Console.ReadLine();
            driver.Quit();
        }

        public static void JobSite(string jobname)
        {
            IWebDriver driver = new ChromeDriver();
       
            List<Dictionary<string, string>> dataList = new List<Dictionary<string, string>>();

            driver.Navigate().GoToUrl("https://www.ictjob.be/");

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            var elementsWithSearchID = wait.Until((driver) => driver.FindElements(By.Id("keywords-input")));
            var search = elementsWithSearchID.Where(e => e.TagName == "input").FirstOrDefault();

            search.SendKeys(jobname);

            var searchBut = driver.FindElement(By.XPath("//*[@id=\"main-search-button\"]"));
            searchBut.Submit();

            Thread.Sleep(40000);

            var cookiesMessage = driver.FindElement(By.XPath("//*[@id=\"body-ictjob\"]/div[2]/a"));
            cookiesMessage.Click();

            var date = driver.FindElement(By.XPath("/html/body/section/div[1]/div/div[2]/div/div/form/div[2]/div/div/div[2]/section/div/div[1]/div[2]/div/div[2]/span[2]/a"));
            date.Click();


            Thread.Sleep(20000);

            int count = 1;

            while (count < 7)
            {
                if (count == 4)
                {
                    count += 1;
                }
                Console.WriteLine("");
                string title = driver.FindElement(By.XPath("/html/body/section/div[1]/div/div[2]/div/div/form/div[2]/div/div/div[2]/section/div/div[2]/div[1]/div/ul/li[" + count + "]/span[2]/a/h2")).Text;
                string company = driver.FindElement(By.XPath("/html/body/section/div[1]/div/div[2]/div/div/form/div[2]/div/div/div[2]/section/div/div[2]/div[1]/div/ul/li[" + count + "]/span[2]/span[1]")).Text;
                string location = driver.FindElement(By.XPath("/html/body/section/div[1]/div/div[2]/div/div/form/div[2]/div/div/div[2]/section/div/div[2]/div[1]/div/ul/li[" + count + "]/span[2]/span[2]/span[2]/span/span")).Text;
                string keyword = driver.FindElement(By.XPath("/html/body/section/div[1]/div/div[2]/div/div/form/div[2]/div/div/div[2]/section/div/div[2]/div[1]/div/ul/li[" + count + "]/span[2]/span[3]")).Text;
                string link = driver.FindElement(By.XPath("/html/body/section/div[1]/div/div[2]/div/div/form/div[2]/div/div/div[2]/section/div/div[2]/div[1]/div/ul/li[" + count + "]/span[2]/a")).GetAttribute("href");

                Console.WriteLine("Title: " + title);
                Console.WriteLine("Company: " + company);
                Console.WriteLine("Located in: " + location);
                Console.WriteLine("Keywords: " + keyword);
                Console.WriteLine("Link: " + link);
                addToJson("Job Title", title, true, false);
                addToJson("Company", company, false, false);
                addToJson("Location", location, false, false);
                addToJson("Keywords", keyword, false, false);
                addToJson("Link", link, false, true);

                addRecordToCSV(title, company, keyword, location, link, "jobs.csv");
                count++;
            }

            
            Console.ReadLine();
            driver.Quit();
        }



    }
}