-- --------------------------------------------------------
-- Host:                         127.0.0.1
-- Server version:               10.9.8-MariaDB - mariadb.org binary distribution
-- Server OS:                    Win64
-- HeidiSQL Version:             12.5.0.6677
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;


-- Dumping database structure for pfantua
CREATE DATABASE IF NOT EXISTS `pfantua` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci */;
USE `pfantua`;

-- Dumping structure for table pfantua.activity
CREATE TABLE IF NOT EXISTS `activity` (
  `ActivityID` int(11) NOT NULL AUTO_INCREMENT COMMENT '流水號',
  `ActivityType` int(11) DEFAULT 1 COMMENT '活動資訊類型(1 : 照片集，2 : 影片)',
  `Title` varchar(200) DEFAULT NULL COMMENT '活動資訊標題',
  `Content` text DEFAULT NULL COMMENT '活動資訊內容',
  `IsActive` bit(1) NOT NULL DEFAULT b'1' COMMENT '是否啟用  (0 : 停用，1 :啟用)',
  `IsDeleted` bit(1) NOT NULL DEFAULT b'0' COMMENT '是否刪除(0:未刪除，1 : 已刪除)',
  `Sort` int(11) NOT NULL DEFAULT 1 COMMENT '排序',
  `CreateTime` datetime NOT NULL COMMENT '建立時間',
  `UpdateTime` datetime DEFAULT NULL COMMENT '最後更新時間',
  PRIMARY KEY (`ActivityID`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Data exporting was unselected.

-- Dumping structure for table pfantua.activityimage
CREATE TABLE IF NOT EXISTS `activityimage` (
  `ActivityImageID` int(11) NOT NULL AUTO_INCREMENT COMMENT '活動資訊照片流水號',
  `ActivityID` int(11) NOT NULL COMMENT '活動資訊流水號',
  `Name` varchar(500) DEFAULT NULL COMMENT '圖片名稱',
  `ImageUrl` varchar(500) DEFAULT NULL COMMENT '圖片路徑',
  `IsActive` bit(1) NOT NULL DEFAULT b'1' COMMENT '是否啟用  (0 : 停用，1 :啟用)',
  `Sort` int(11) NOT NULL DEFAULT 1 COMMENT '排序',
  `CreateTime` datetime NOT NULL COMMENT '建立時間',
  `UpdateTime` datetime DEFAULT NULL COMMENT '最後更新時間',
  PRIMARY KEY (`ActivityImageID`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=36 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Data exporting was unselected.

-- Dumping structure for table pfantua.activityvideo
CREATE TABLE IF NOT EXISTS `activityvideo` (
  `ActivityVideoID` int(11) NOT NULL AUTO_INCREMENT COMMENT '活動資訊影音連結流水號',
  `ActivityID` int(11) NOT NULL COMMENT '活動資訊流水號',
  `Name` varchar(500) DEFAULT NULL COMMENT '影片名稱',
  `ImageUrl` varchar(250) DEFAULT NULL COMMENT '圖片路徑',
  `VideoUrl` varchar(250) DEFAULT NULL COMMENT '影片路徑',
  `IsActive` bit(1) NOT NULL DEFAULT b'1' COMMENT '是否啟用  (0 : 停用，1 :啟用)',
  `Sort` int(11) NOT NULL DEFAULT 1 COMMENT '排序',
  `CreateTime` datetime NOT NULL COMMENT '建立時間',
  `UpdateTime` datetime DEFAULT NULL COMMENT '最後更新時間',
  PRIMARY KEY (`ActivityVideoID`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=14 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Data exporting was unselected.

-- Dumping structure for table pfantua.banner
CREATE TABLE IF NOT EXISTS `banner` (
  `BannerID` int(11) NOT NULL AUTO_INCREMENT COMMENT '流水號',
  `BannerName` varchar(200) DEFAULT NULL COMMENT 'Banner 名稱',
  `BannerType` int(11) NOT NULL DEFAULT 1 COMMENT 'Banner 類型',
  `ImageUrl` varchar(250) DEFAULT NULL COMMENT '圖片路徑',
  `TargetUrl` varchar(250) DEFAULT NULL COMMENT '目標路徑',
  `IsActive` bit(1) NOT NULL DEFAULT b'1' COMMENT '是否啟用  (0 : 停用，1 :啟用)',
  `IsDeleted` bit(1) DEFAULT b'0' COMMENT '是否刪除 (0 : 未刪除，1 :已刪除)',
  `Sort` int(11) NOT NULL DEFAULT 1 COMMENT '排序',
  `CreateTime` datetime NOT NULL COMMENT '建立時間',
  `UpdateTime` datetime DEFAULT NULL COMMENT '最後更新時間',
  PRIMARY KEY (`BannerID`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Data exporting was unselected.

-- Dumping structure for table pfantua.collagecolleague
CREATE TABLE IF NOT EXISTS `collagecolleague` (
  `CollageColleagueID` int(11) NOT NULL AUTO_INCREMENT COMMENT '系所同事流水號',
  `CollageCode` varchar(20) DEFAULT NULL COMMENT '學院代號',
  `CollageColleagueCode` varchar(20) NOT NULL COMMENT '系所同事代號',
  `Name` varchar(200) DEFAULT NULL COMMENT '老師名字',
  `ImageUrl` varchar(300) DEFAULT NULL COMMENT '同事照片連結',
  `Title` text DEFAULT NULL COMMENT '現職抬頭',
  `Content` text DEFAULT NULL COMMENT '簡介',
  `PositionType` varchar(50) DEFAULT NULL COMMENT '職稱類別(`professor`, `associate-professor`, `assistant-professor `, `lecturer`, `administrative-staff`, `assistant`)',
  `Email` varchar(250) DEFAULT NULL COMMENT '聯絡信箱',
  `Phone` varchar(200) DEFAULT NULL COMMENT '連絡電話',
  `Sort` int(11) NOT NULL DEFAULT 1 COMMENT '排序',
  `IsHead` bit(1) NOT NULL DEFAULT b'1' COMMENT '是否是院長  (0 : 不是，1 : 是)',
  `IsActive` bit(1) NOT NULL DEFAULT b'1' COMMENT '是否啟用  (0 : 停用，1 :啟用)',
  `IsDeleted` bit(1) DEFAULT b'0' COMMENT '是否刪除(0 : 未刪除，1:已刪除)',
  `CreateTime` datetime NOT NULL COMMENT '建立時間',
  `UpdateTime` datetime DEFAULT NULL COMMENT '最後更新時間',
  PRIMARY KEY (`CollageColleagueID`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=14 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Data exporting was unselected.

-- Dumping structure for table pfantua.collagecourse
CREATE TABLE IF NOT EXISTS `collagecourse` (
  `CollageCourseID` int(11) NOT NULL AUTO_INCREMENT COMMENT '系所課程流水號',
  `CollageCode` varchar(20) DEFAULT NULL COMMENT '學院代號',
  `CollageDepartmentCode` varchar(50) DEFAULT NULL COMMENT '學院系所代號',
  `CollageCourseCode` varchar(20) NOT NULL COMMENT '系所課程代號',
  `Title` varchar(200) DEFAULT NULL COMMENT '系所課程標題',
  `Content` text DEFAULT NULL COMMENT '系所課程簡介',
  `Url` varchar(500) DEFAULT NULL COMMENT '系所課程連結',
  `Sort` int(11) NOT NULL DEFAULT 1 COMMENT '排序',
  `IsActive` bit(1) NOT NULL DEFAULT b'1' COMMENT '是否啟用  (0 : 停用，1 :啟用)',
  `IsDeleted` bit(1) DEFAULT b'0' COMMENT '是否刪除(0 : 未刪除，1:已刪除)',
  `CreateTime` datetime NOT NULL COMMENT '建立時間',
  `UpdateTime` datetime DEFAULT NULL COMMENT '最後更新時間',
  PRIMARY KEY (`CollageCourseID`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=14 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Data exporting was unselected.

-- Dumping structure for table pfantua.collagedepartment
CREATE TABLE IF NOT EXISTS `collagedepartment` (
  `CollageDepartmentID` int(11) NOT NULL AUTO_INCREMENT COMMENT '系所流水號',
  `CollageCode` varchar(20) DEFAULT NULL COMMENT '學院代號',
  `CollageDepartmentCode` varchar(20) NOT NULL COMMENT '系所代號',
  `CollageDepartmentLinkUrl` varchar(300) DEFAULT NULL COMMENT '系所連結',
  `ImageUrl` varchar(500) DEFAULT NULL COMMENT '系所圖片',
  `Name` varchar(200) DEFAULT NULL COMMENT '系所名稱',
  `Content` text DEFAULT NULL COMMENT '簡介',
  `Sort` int(11) NOT NULL DEFAULT 1 COMMENT '排序',
  `IsActive` bit(1) NOT NULL DEFAULT b'1' COMMENT '是否啟用  (0 : 停用，1 :啟用)',
  `IsDeleted` bit(1) DEFAULT b'0' COMMENT '是否刪除(0 : 未刪除，1:已刪除)',
  `CreateTime` datetime NOT NULL COMMENT '建立時間',
  `UpdateTime` datetime DEFAULT NULL COMMENT '最後更新時間',
  PRIMARY KEY (`CollageDepartmentID`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Data exporting was unselected.

-- Dumping structure for table pfantua.collagedepartmentcolleaguemapping
CREATE TABLE IF NOT EXISTS `collagedepartmentcolleaguemapping` (
  `CollageDepartmentCode` varchar(20) NOT NULL COMMENT '系所代號',
  `CollageColleagueCode` varchar(20) NOT NULL COMMENT '系所老師代號',
  `IsActive` bit(1) NOT NULL DEFAULT b'1' COMMENT '是否啟用  (0 : 停用，1 :啟用)',
  `CreateTime` datetime NOT NULL COMMENT '建立時間',
  `UpdateTime` datetime DEFAULT NULL COMMENT '最後更新時間',
  PRIMARY KEY (`CollageDepartmentCode`,`CollageColleagueCode`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Data exporting was unselected.

-- Dumping structure for table pfantua.collageheadelection
CREATE TABLE IF NOT EXISTS `collageheadelection` (
  `CollageHeadElectionID` int(11) NOT NULL AUTO_INCREMENT COMMENT '學院院長選舉編號',
  `Title` varchar(500) DEFAULT NULL COMMENT '學院院長選舉標題',
  `Content` text DEFAULT NULL COMMENT '學院院長選舉內容',
  `Year` year(4) NOT NULL COMMENT '年分',
  `CreateTime` datetime NOT NULL COMMENT '建立時間',
  `UpdateTime` datetime DEFAULT NULL COMMENT '最後更新時間',
  `IsActive` bit(1) DEFAULT b'1' COMMENT '是否啟用(0 : 停用，1: 啟用)',
  `IsDeleted` bit(1) DEFAULT b'0' COMMENT '是否刪除(0 : 未刪除，1:已刪除)',
  PRIMARY KEY (`CollageHeadElectionID`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Data exporting was unselected.

-- Dumping structure for table pfantua.collageheadelectionfile
CREATE TABLE IF NOT EXISTS `collageheadelectionfile` (
  `CollageHeadElectionFileID` int(11) NOT NULL AUTO_INCREMENT COMMENT '學院院長選舉檔案編號',
  `CollageHeadElectionID` int(11) NOT NULL COMMENT '學院院長選舉編號',
  `Name` varchar(500) DEFAULT NULL COMMENT '名稱',
  `Url` varchar(250) DEFAULT NULL COMMENT '檔案路徑',
  `IsActive` bit(1) NOT NULL DEFAULT b'0' COMMENT '是否啟用(0 : 停用，1: 啟用)',
  `CreateTime` datetime DEFAULT NULL COMMENT '建立時間',
  `UpdateTime` datetime DEFAULT NULL COMMENT '最後更新時間',
  PRIMARY KEY (`CollageHeadElectionFileID`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=40 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Data exporting was unselected.

-- Dumping structure for table pfantua.collageheadhistory
CREATE TABLE IF NOT EXISTS `collageheadhistory` (
  `CollageColleagueCode` varchar(20) NOT NULL COMMENT '系所老師代號',
  `Year` year(4) NOT NULL COMMENT '年分',
  `CreateTime` datetime NOT NULL COMMENT '建立時間',
  PRIMARY KEY (`CollageColleagueCode`,`Year`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Data exporting was unselected.

-- Dumping structure for table pfantua.collageinfo
CREATE TABLE IF NOT EXISTS `collageinfo` (
  `CollageID` int(11) NOT NULL AUTO_INCREMENT COMMENT '學院編號',
  `CollageCode` varchar(50) DEFAULT NULL COMMENT '學院代號',
  `CollageName` varchar(100) DEFAULT NULL COMMENT '學院名稱',
  `CollageImageUrl` varchar(200) DEFAULT NULL COMMENT '學院圖示',
  `CollageIntroduction` text DEFAULT NULL COMMENT '學院簡介',
  `CollageHistory` text DEFAULT NULL COMMENT '學院歷史',
  `CreateTime` datetime DEFAULT NULL COMMENT '建立時間',
  `UpdateTime` datetime DEFAULT NULL COMMENT '最後更新時間',
  PRIMARY KEY (`CollageID`)
) ENGINE=MyISAM AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci COMMENT='學院資訊';

-- Data exporting was unselected.

-- Dumping structure for table pfantua.news
CREATE TABLE IF NOT EXISTS `news` (
  `NewsID` int(11) NOT NULL AUTO_INCREMENT COMMENT '流水號',
  `NewsCategoryID` int(11) NOT NULL DEFAULT 1 COMMENT '最新消息分類流水號',
  `Title` varchar(200) DEFAULT NULL COMMENT '最新消息標題',
  `Content` text DEFAULT NULL COMMENT '最新消息內容',
  `ImageUrl` varchar(250) DEFAULT NULL COMMENT '圖片路徑',
  `TargetUrl` varchar(250) DEFAULT NULL COMMENT '目標路徑',
  `IsActive` bit(1) NOT NULL DEFAULT b'1' COMMENT '是否啟用  (0 : 停用，1 :啟用)',
  `IsDeleted` bit(1) DEFAULT b'0' COMMENT '是否刪除(0 : 未刪除，1:已刪除)',
  `IsNew` bit(1) DEFAULT b'0' COMMENT '是否為最新(0 : 不是，1:是)',
  `Sort` int(11) NOT NULL DEFAULT 1 COMMENT '排序',
  `CreateTime` datetime NOT NULL COMMENT '建立時間',
  `UpdateTime` datetime DEFAULT NULL COMMENT '最後更新時間',
  `NewsTime` date DEFAULT NULL COMMENT '消息時間',
  PRIMARY KEY (`NewsID`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=18 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Data exporting was unselected.

-- Dumping structure for table pfantua.newscategory
CREATE TABLE IF NOT EXISTS `newscategory` (
  `NewsCategoryID` int(11) NOT NULL AUTO_INCREMENT COMMENT '最新消息分類流水號',
  `Name` varchar(200) DEFAULT NULL COMMENT '相關資源分類名稱',
  `Sort` int(11) NOT NULL DEFAULT 1 COMMENT '排序',
  PRIMARY KEY (`NewsCategoryID`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Data exporting was unselected.

-- Dumping structure for table pfantua.regulation
CREATE TABLE IF NOT EXISTS `regulation` (
  `RegulationID` int(11) NOT NULL AUTO_INCREMENT COMMENT '法規章程流水號',
  `Title` varchar(200) DEFAULT NULL COMMENT '法規章程標題',
  `Content` text DEFAULT NULL COMMENT '法規章程內容',
  `FileUrl` varchar(200) DEFAULT NULL COMMENT '法規章程檔案連結',
  `Sort` int(11) NOT NULL DEFAULT 1 COMMENT '排序',
  `IsActive` bit(1) NOT NULL DEFAULT b'1' COMMENT '是否啟用  (0 : 停用，1 :啟用)',
  `IsDeleted` bit(1) DEFAULT b'0' COMMENT '是否刪除(0 : 未刪除，1:已刪除)',
  `CreateTime` datetime NOT NULL COMMENT '建立時間',
  `UpdateTime` datetime DEFAULT NULL COMMENT '最後更新時間',
  PRIMARY KEY (`RegulationID`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Data exporting was unselected.

-- Dumping structure for table pfantua.relatedresource
CREATE TABLE IF NOT EXISTS `relatedresource` (
  `RelatedResourceID` int(11) NOT NULL AUTO_INCREMENT COMMENT '相關資源流水號',
  `RelatedResourceCategoryID` int(11) NOT NULL COMMENT '相關資源分類流水號(1 : 行政資訊，2 : 特殊活動，3 : 學生專區)',
  `Title` varchar(200) DEFAULT NULL COMMENT '標題',
  `Content` text DEFAULT NULL COMMENT '內容',
  `RelatedResourceUrl` varchar(200) DEFAULT NULL COMMENT '相關資源 Url',
  `IsActive` bit(1) NOT NULL DEFAULT b'1' COMMENT '是否啟用  (0 : 停用，1 :啟用)',
  `IsDeleted` bit(1) DEFAULT b'0' COMMENT '是否刪除(0 : 未刪除，1:已刪除)',
  `Sort` int(11) NOT NULL DEFAULT 1 COMMENT '排序',
  `CreateTime` datetime NOT NULL COMMENT '建立時間',
  `UpdateTime` datetime DEFAULT NULL COMMENT '最後更新時間',
  PRIMARY KEY (`RelatedResourceID`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=22 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Data exporting was unselected.

-- Dumping structure for table pfantua.relatedresourcecategory
CREATE TABLE IF NOT EXISTS `relatedresourcecategory` (
  `RelatedResourceCategoryID` int(11) NOT NULL AUTO_INCREMENT COMMENT '相關資源分類流水號(1 : 行政資訊，2 : 特殊活動，3 : 學生專區)',
  `Key` varchar(50) DEFAULT NULL COMMENT '相關資源Key',
  `Name` varchar(200) DEFAULT NULL COMMENT '相關資源分類名稱',
  `Sort` int(11) NOT NULL DEFAULT 1 COMMENT '排序',
  PRIMARY KEY (`RelatedResourceCategoryID`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Data exporting was unselected.

-- Dumping structure for table pfantua.role
CREATE TABLE IF NOT EXISTS `role` (
  `RoleID` int(11) NOT NULL AUTO_INCREMENT,
  `RoleName` varchar(50) NOT NULL,
  `Function` longtext NOT NULL DEFAULT '',
  `Enable` bit(1) NOT NULL DEFAULT b'1',
  PRIMARY KEY (`RoleID`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci COMMENT='後台使用者群組';

-- Data exporting was unselected.

-- Dumping structure for table pfantua.systemlog
CREATE TABLE IF NOT EXISTS `systemlog` (
  `ID` int(11) NOT NULL AUTO_INCREMENT COMMENT '流水號',
  `Account` varchar(200) DEFAULT NULL COMMENT '帳號',
  `IpAddress` varchar(200) DEFAULT NULL COMMENT 'IP',
  `Action` varchar(50) DEFAULT NULL COMMENT '操作代碼',
  `ActionObject` text DEFAULT NULL COMMENT '操作物件',
  `Content` text DEFAULT NULL COMMENT '內容',
  `CreateTime` datetime DEFAULT NULL COMMENT '建立時間',
  `Success` bit(1) DEFAULT b'1' COMMENT '是否成功(1 : 成功, 0 : 失敗)',
  PRIMARY KEY (`ID`)
) ENGINE=MyISAM AUTO_INCREMENT=273 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci COMMENT='系統紀錄表';

-- Data exporting was unselected.

-- Dumping structure for table pfantua.user
CREATE TABLE IF NOT EXISTS `user` (
  `UserID` int(11) NOT NULL AUTO_INCREMENT COMMENT '流水號',
  `UserName` varchar(50) NOT NULL COMMENT '帳號',
  `Password` varchar(300) NOT NULL DEFAULT '' COMMENT '密碼',
  `Name` varchar(50) DEFAULT NULL COMMENT '名稱',
  `Email` varchar(250) DEFAULT NULL COMMENT '信箱',
  `IsActive` bit(1) NOT NULL DEFAULT b'1' COMMENT '是否啟用  (0 : 停用，1 :啟用)',
  `RoleID` int(11) NOT NULL DEFAULT 0 COMMENT '角色編號',
  `CreateTime` datetime NOT NULL COMMENT '建立時間',
  `LastLoginTime` datetime NOT NULL COMMENT '最後登入時間',
  `LastUpdatePasswordTime` datetime DEFAULT NULL COMMENT '密碼最後更新時間',
  `LoginErrorCount` int(1) DEFAULT 0 COMMENT '登入錯誤次數(大於三次須鎖定15分鐘)',
  `IsDefaultPassword` bit(1) DEFAULT b'1' COMMENT '是否為預設密碼',
  `IsDeleted` bit(1) DEFAULT b'0' COMMENT '是否刪除',
  PRIMARY KEY (`UserID`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci COMMENT='後臺使用者';

-- Data exporting was unselected.

/*!40103 SET TIME_ZONE=IFNULL(@OLD_TIME_ZONE, 'system') */;
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;
