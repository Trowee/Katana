#!/bin/bash

cp .gitattributes SourceAssets/.gitattributes
cd SourceAssets
git add .gitattributes
git commit -m "Sync .gitattributes from parent repo"
cd ..
