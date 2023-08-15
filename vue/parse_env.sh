#!/bin/bash

cp env-config_exp.js  env-config.js 

export API_URL=$(echo "$API_URL" | sed 's/\//\\\//g' )

sed "s/API_URL/$API_URL/g" -i env-config.js 
