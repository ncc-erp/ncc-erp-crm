#!/bin/bash

cp appsettings_exp.json  appsettings.json 

export API_URL=$(echo "$API_URL" | sed 's/\//\\\//g' )
export URL=$(echo "$URL" | sed 's/\//\\\//g' )
export API_BE=$(echo "$API_BE" | sed 's/\//\\\//g' )

sed "s/API_URL/$API_URL/g" -i appsettings.json
sed "s/URL/$URL/g" -i appsettings.json
sed "s/DB_HOST/$DB_HOST/g" -i appsettings.json
sed "s/DB_NAME/$DB_NAME/g" -i appsettings.json
sed "s/DB_PASS/$DB_PASS/g" -i appsettings.json
sed "s/DB_USER/$DB_USER/g" -i appsettings.json
sed "s/API_BE/$API_BE/g" -i appsettings.json
sed "s/SEC_KEY/$SEC_KEY/g" -i appsettings.json
