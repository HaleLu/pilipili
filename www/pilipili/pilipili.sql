/*
Navicat MySQL Data Transfer

Source Server         : remote
Source Server Version : 50545
Source Host           : ap-cdbr-azure-east-c.cloudapp.net:3306
Source Database       : acsm_921d45dd955c251

Target Server Type    : MYSQL
Target Server Version : 50545
File Encoding         : 65001

Date: 2016-06-20 02:10:15
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for pilipili
-- ----------------------------
DROP TABLE IF EXISTS `pilipili`;
CREATE TABLE `pilipili` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `roomId` int(11) NOT NULL,
  `name` varchar(64) NOT NULL,
  `content` text NOT NULL,
  `time` int(11) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM AUTO_INCREMENT=383 DEFAULT CHARSET=utf8;
