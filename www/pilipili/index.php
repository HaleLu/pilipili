<?php

require_once 'init.inc.php';

if(isset($_GET['id']) && isset($_GET['roomId'])){
	$lastId = $_GET['id'];
	$roomId = $_GET['roomId'];
	//请求返回数组
	$return = array();
	//数据库连接
	$stmt = $GLOBALS['db']->prepare("SELECT `id`, `name`, `content`, `time` FROM `pilipili` WHERE `id` > ? AND `roomId` = ?");
	$stmt->bind_param('ii', $lastId, $roomId);
	$stmt->execute();
	$stmt->bind_result($id,$name,$cont,$time);
	$return['data'] = array();
	while ($stmt->fetch()) {
		$return['data'][] = array(
			'id' => $id,
			'name' => $name,
			'content' => $cont,
			'time' =>  date('Y-m-d,H:i:s', $time)
		);
	}
	//GET请求后清空数据表
	$stmt = $GLOBALS['db']->prepare("DELETE FROM `pilipili` WHERE `roomId` = ?");
	$stmt->bind_param('i', $roomId);
	$stmt->execute();
	echo json_encode($return);
}else{
	echo "Access Denied!";
}

?>