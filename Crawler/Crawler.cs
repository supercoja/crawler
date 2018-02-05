using System.IO;

using System.Net;


using System;


using System.Text;


namespace Crawler { 
public class Crawler


{


    static void Main()


    {


        //query top X number of urls for keyword


        //Create URL array


        //Create web request from URL array


        WebRequest request = WebRequest.Create("http://engineerverse.com/");


        // If required by the server, set the credentials.


        request.Credentials = CredentialCache.DefaultCredentials;


        // Get the response.


        HttpWebResponse response = (HttpWebResponse)request.GetResponse();


        //Display Status


        Console.WriteLine(response.StatusDescription);


        // Get the stream containing content returned by the server.


        Stream dataStream = response.GetResponseStream();


        // Open the stream using a StreamReader for easy access.


        StreamReader reader = new StreamReader(dataStream);


        // Read the content.


        string responseFromServer = reader.ReadToEnd();


        // Display the content.


        //Console.WriteLine (responseFromServer);


        //write response to textfile


        System.IO.File.WriteAllText(@"C:\dev\Crawler\WriteText.txt", responseFromServer);


        // Cleanup the streams and the response.


        reader.Close();


        dataStream.Close();


        response.Close();


        Console.WriteLine("ready");


        Console.ReadKey();


    }


}
}