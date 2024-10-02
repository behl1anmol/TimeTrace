CREATE TABLE "ConfigurationSetting" (
    "ConfigurationSettingId" INTEGER NOT NULL CONSTRAINT "PK_ConfigurationSetting" PRIMARY KEY AUTOINCREMENT,
    "ConfigurationSettingKey" TEXT NOT NULL,
    "DateTimeStamp" TEXT NOT NULL DEFAULT (CURRENT_TIMESTAMP)
);


CREATE TABLE "Process" (
    "ProcessId" INTEGER NOT NULL CONSTRAINT "PK_Process" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "DateTimeStamp" TEXT NOT NULL DEFAULT (CURRENT_TIMESTAMP)
);


CREATE TABLE "ConfigurationSettingDetail" (
    "ConfigurationSettingDetailId" INTEGER NOT NULL CONSTRAINT "PK_ConfigurationSettingDetail" PRIMARY KEY AUTOINCREMENT,
    "ConfigurationSettingValue" TEXT NULL,
    "ConfigurationSettingValueDescription" TEXT NULL,
    "ConfigurationSettingId" INTEGER NOT NULL,
    "DateTimeStamp" TEXT NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    CONSTRAINT "FK_ConfigurationSettingDetail_ConfigurationSetting_ConfigurationSettingId" FOREIGN KEY ("ConfigurationSettingId") REFERENCES "ConfigurationSetting" ("ConfigurationSettingId") ON DELETE CASCADE
);


CREATE TABLE "ProcessDetail" (
    "ProcessDetailId" INTEGER NOT NULL CONSTRAINT "PK_ProcessDetail" PRIMARY KEY AUTOINCREMENT,
    "Description" TEXT NULL,
    "DateTimeStamp" TEXT NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    "ProcessId" INTEGER NOT NULL,
    CONSTRAINT "FK_ProcessDetail_Process_ProcessId" FOREIGN KEY ("ProcessId") REFERENCES "Process" ("ProcessId") ON DELETE CASCADE
);


CREATE TABLE "Image" (
    "ImageId" INTEGER NOT NULL CONSTRAINT "PK_Image" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "ImagePath" TEXT NOT NULL,
    "DateTimeStamp" TEXT NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    "ProcessDetailId" INTEGER NOT NULL,
    CONSTRAINT "FK_Image_ProcessDetail_ProcessDetailId" FOREIGN KEY ("ProcessDetailId") REFERENCES "ProcessDetail" ("ProcessDetailId") ON DELETE CASCADE
);


CREATE UNIQUE INDEX "IX_ConfigurationSetting_ConfigurationSettingKey" ON "ConfigurationSetting" ("ConfigurationSettingKey");


CREATE INDEX "IX_ConfigurationSettingDetail_ConfigurationSettingId" ON "ConfigurationSettingDetail" ("ConfigurationSettingId");


CREATE UNIQUE INDEX "IX_ConfigurationSettingDetail_ConfigurationSettingValueDescription" ON "ConfigurationSettingDetail" ("ConfigurationSettingValueDescription");


CREATE UNIQUE INDEX "IX_Image_Name" ON "Image" ("Name");


CREATE INDEX "IX_Image_ProcessDetailId" ON "Image" ("ProcessDetailId");


CREATE UNIQUE INDEX "IX_Process_Name" ON "Process" ("Name");


CREATE UNIQUE INDEX "IX_ProcessDetail_Description" ON "ProcessDetail" ("Description");


CREATE INDEX "IX_ProcessDetail_ProcessId" ON "ProcessDetail" ("ProcessId");