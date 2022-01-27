#!/usr/bin/env node

var path = require("path");
var fs = require("fs");

function recFindByExt(base, ext, files, result) {
  files = files || fs.readdirSync(base);
  result = result || [];

  files.forEach(function (file) {
    var newbase = path.join(base, file);
    if (fs.statSync(newbase).isDirectory()) {
      result = recFindByExt(newbase, ext, fs.readdirSync(newbase), result);
    } else {
      if (file === ext && newbase.indexOf("/bin/") == -1) {
        result.push(newbase);
      }
    }
  });
  return result;
}

var settingsFiles = recFindByExt(".", "local.settings.json");

for (var i = 0; i < settingsFiles.length; i++) {
  var filePath = settingsFiles[i];
  var jsonObj = JSON.parse(fs.readFileSync(filePath, "utf8"));
  var values = jsonObj.Values;
  var transformed = Object.entries(values)
    .map(([key, value]) => ({
      name: key.replace(":", "__"),
      value,
      slotSetting: false,
    }))
    .filter(
      (item) =>
        ![
          "AzureWebJobsStorage",
          "_AzureWebJobsStorage",
          "FUNCTIONS_WORKER_RUNTIME",
        ].includes(item.name)
    );
  console.log();
  console.log(filePath);
  console.log(JSON.stringify(transformed, null, 2));
}
