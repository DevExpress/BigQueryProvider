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

Download via <a href="https://www.nuget.org/packages/DevExpress.DataAccess.BigQuery">Nuget</a>

###About connection string

For connection to server you need:
  <ul>
    <li><b>ProjectId</b> - It is written in your <a href="https://console.developers.google.com/project">console</a> next to the name of the project.</li>
    <li><b>DataSetId</b> - It is written in your project.</li>
    <li><b>OAuthClientId</b> and <b>OAuthClientSecret</b> - It is written in APIs&auth/Credentials. You need to create new credentials for "Installed application".</li>
    <li><b>OAuthRefreshToken</b> - You can get it <a href="https://developers.google.com/oauthplayground">here</a>.</li>
  </ul>
  
  Example connection string:
  
  ```xml
  ProjectID=projectID;DataSetId=dataSetId;OAuthClientId=oAuthClientId;OAuthClientSecret=oAuthClientSecret;OAuthRefreshToken=oAuthRefreshToken
  ```

