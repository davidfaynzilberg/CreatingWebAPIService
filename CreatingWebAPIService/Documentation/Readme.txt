Create d the app using C# because I do have the environment ready.
To test the API used Postman
Postman script attached

URI: http://localhost:56290/api/Home/CheckCountryEnableability
Body -> raw -> JSON - Example
{
	"SiteApiKey": "1234-5678-9010-3232",
	"Ip": "24.125.27.4",
    "WhiteList": 
        [
        	{"CountryName": "China"},
        	{"CountryName": "United States"},
        	{"CountryName": "Russia"},
        	{"CountryName": "ggggggggggggggggg"}
        ]
}

Expected results true/false

For security reasons we do not provide any additional information to the subscriber, however logging all errors.

Spent 5.45 hours developing, most of the time spent on finding the Geo file

Keeping the data up to date is a part of the solution. All global variables are set in web-config, for example: GeoLite2-Country-Days, GeoLiteFileName etc...

mmdb file link: https://geolite.maxmind.com/download/geoip/database/GeoLite2-Country.tar.gz but it is also in the web.config file.

Ideally I would store all the secret in KeyWalt(Azure) or AWS Secrets(AWS)

