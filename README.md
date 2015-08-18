# BigQueryProvider

<a href="http://dataaccess.cloudapp.net:9999/viewType.html?buildTypeId=BigQueryProvider_DebugTest&guest=1">
<img src="http://img.shields.io/teamcity/http/dataaccess.cloudapp.net:9999/s/BigQueryProvider_DebugTest.svg?style=flat&label=DebugTest"/>
</a>
<a href="http://dataaccess.cloudapp.net:9999/viewType.html?buildTypeId=BigQueryProvider_Releas&guest=1">
<img src="http://img.shields.io/teamcity/http/dataaccess.cloudapp.net:9999/s/BigQueryProvider_Release.svg?style=flat&label=Release"/>
</a>
<a href="https://www.nuget.org/packages/DevExpress.DataAccess.BigQuery">
<img src="https://img.shields.io/nuget/v/DevExpress.DataAccess.BigQuery.svg?style=flat"/>
</a>

BigQueryProvider is a .NET data provider for Google <a href="https://cloud.google.com/bigquery/">BigQuery</a>. It allows you to connect and interact with Google BigQuery server using .NET. 

Here’s a basic code snippet to get started.:
  
  ```C#
using (var connection = new BigQueryConnection("ProjectID=myProject;DataSetId=myDataSet;
OAuthClientId=myClientId;OAuthClientSecret=myClientSecret;OAuthRefreshToken=myRefreshToken")){
    connection.Open();
    using (var command = new BigQueryCommand()) {
        command.Connection = connection;

        // Retrieve all rows
        command.CommandText = "SELECT my_field FROM my_data";
        using (var reader = command.ExecuteReader()) {
            while (reader.Read()) {
                Console.WriteLine(reader.GetString(0));
            }
        }
    }
}
  
  ```
