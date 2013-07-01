SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='TRADITIONAL,ALLOW_INVALID_DATES';

DROP SCHEMA IF EXISTS `useragentv2_db` ;
CREATE SCHEMA IF NOT EXISTS `useragentv2_db` DEFAULT CHARACTER SET utf8 ;
USE `useragentv2_db` ;

-- -----------------------------------------------------
-- Table `useragentv2_db`.`pulse_account`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `useragentv2_db`.`pulse_account` ;

CREATE  TABLE IF NOT EXISTS `useragentv2_db`.`pulse_account` (
  `account_id` BIGINT(20) NOT NULL AUTO_INCREMENT ,
  `account_name` CHAR(32) NOT NULL ,
  `account_pass` CHAR(64) NOT NULL ,
  `account_email` CHAR(64) NOT NULL ,
  `account_nickname` CHAR(16) NOT NULL ,
  `account_regtime` INT(11) NOT NULL ,
  `account_lastlogin` INT(11) NOT NULL ,
  `account_recharge` INT(11) NOT NULL DEFAULT '0' COMMENT '累计充值' ,
  PRIMARY KEY (`account_id`) ,
  INDEX `account_name` (`account_name` ASC, `account_pass` ASC) )
ENGINE = MyISAM
DEFAULT CHARACTER SET = utf8;


-- -----------------------------------------------------
-- Table `useragentv2_db`.`pulse_log`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `useragentv2_db`.`pulse_log` ;

CREATE  TABLE IF NOT EXISTS `useragentv2_db`.`pulse_log` (
  `log_id` BIGINT(20) NOT NULL AUTO_INCREMENT ,
  `log_type` CHAR(32) NOT NULL ,
  `log_account_name` CHAR(32) NOT NULL ,
  `log_uri` CHAR(128) NOT NULL ,
  `log_method` CHAR(8) NOT NULL ,
  `log_parameter` TEXT NOT NULL ,
  `log_time_local` DATETIME NOT NULL ,
  PRIMARY KEY (`log_id`) )
ENGINE = MyISAM
DEFAULT CHARACTER SET = utf8;


-- -----------------------------------------------------
-- Table `useragentv2_db`.`pulse_products`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `useragentv2_db`.`pulse_products` ;

CREATE  TABLE IF NOT EXISTS `useragentv2_db`.`pulse_products` (
  `product_id` INT(11) NOT NULL AUTO_INCREMENT ,
  `product_name` CHAR(16) NOT NULL COMMENT '游戏名称' ,
  `product_url_website` CHAR(128) NOT NULL COMMENT '游戏官网链接' ,
  `product_status` ENUM('PUBLIC','BETA','HOT') NOT NULL DEFAULT 'PUBLIC' ,
  `product_exchange_rate` INT(11) NOT NULL COMMENT '1元人民币能兑换多少游戏币' ,
  `product_currency_name` CHAR(8) NOT NULL ,
  `product_server_role` CHAR(64) NOT NULL COMMENT '获取角色列表的接口' ,
  `product_server_recharge` CHAR(64) NOT NULL COMMENT '充值的接口' ,
  `product_key` CHAR(32) NOT NULL ,
  PRIMARY KEY (`product_id`) )
ENGINE = MyISAM
AUTO_INCREMENT = 1001
DEFAULT CHARACTER SET = utf8;


-- -----------------------------------------------------
-- Table `useragentv2_db`.`pulse_serverlist`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `useragentv2_db`.`pulse_serverlist` ;

CREATE  TABLE IF NOT EXISTS `useragentv2_db`.`pulse_serverlist` (
  `product_id` INT(11) NOT NULL ,
  `server_id` INT(11) NOT NULL ,
  `server_name` CHAR(32) NOT NULL ,
  `server_time_start` INT(11) NOT NULL ,
  `server_status` ENUM('NORMAL','LOW','HOT','CLOSE') NOT NULL DEFAULT 'NORMAL' ,
  `server_web_ip` CHAR(16) NOT NULL ,
  `server_web_port` INT NOT NULL ,
  `server_secure_port` INT NOT NULL ,
  `server_game_ip` CHAR(16) NOT NULL ,
  `server_game_port` CHAR(10) NOT NULL ,
  `server_sort` INT NOT NULL DEFAULT 0 ,
  `server_recommand` INT NOT NULL DEFAULT 0 ,
  PRIMARY KEY (`product_id`, `server_id`) )
ENGINE = MyISAM
DEFAULT CHARACTER SET = utf8;


-- -----------------------------------------------------
-- Table `useragentv2_db`.`pulse_server_status`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `useragentv2_db`.`pulse_server_status` ;

CREATE  TABLE IF NOT EXISTS `useragentv2_db`.`pulse_server_status` (
  `id` INT NOT NULL AUTO_INCREMENT ,
  `server_status` INT NOT NULL DEFAULT 1 ,
  `closed_message` TEXT NOT NULL ,
  `redirect_url` TEXT NOT NULL ,
  PRIMARY KEY (`id`) )
ENGINE = InnoDB;

USE `useragentv2_db` ;


SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
