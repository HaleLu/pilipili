<?php

/**
* 封装好的数据库类
*/
class DB {
	private $db;
	function __construct($host, $username, $password, $database) {
		$this->db = new mysqli($host, $username, $password, $database) or die('数据库连接错误！');
	}
	function prepare($sql) {
		return $this->db->prepare($sql);
	}
}

?>