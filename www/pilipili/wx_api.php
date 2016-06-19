<?php

require_once 'init.inc.php';

// //微信验证
// define("TOKEN", "pilipili");
$wechatObj = new wechatCallback();
// $wechatObj->valid();
// //微信自动回复
$wechatObj->responseMsg();


class wechatCallback
{
    public $fromUsername;
    public $time;
    public $keyword;
    private $appid = 'wxfb056bf940debab2';
    private $appsecret = '99c8349844d47bd6c04102b27327732a';
    private $istoolong = false;

	public function valid()
    {
        $echoStr = $_GET["echostr"];

        //valid signature , option
        if($this->checkSignature())
        {
        	echo $echoStr;
        	exit;
        }
    }

    public function responseMsg()
    {
		//get post data, May be due to the different environments
		$postStr = $GLOBALS["HTTP_RAW_POST_DATA"];

      	//extract post data
		if (!empty($postStr))
        {
            /* libxml_disable_entity_loader is to prevent XML eXternal Entity Injection,
               the best way is to check the validity of xml by yourself */
            libxml_disable_entity_loader(true);
          	$postObj = simplexml_load_string($postStr, 'SimpleXMLElement', LIBXML_NOCDATA);
            $fromUsername = $postObj->FromUserName;
            $toUsername = $postObj->ToUserName;
            $keyword = trim($postObj->Content);
            $istoolong = (strlen($keyword) > 60);
            $time = time();
            $textTpl = "<xml>
						<ToUserName><![CDATA[%s]]></ToUserName>
						<FromUserName><![CDATA[%s]]></FromUserName>
						<CreateTime>%s</CreateTime>
						<MsgType><![CDATA[%s]]></MsgType>
						<Content><![CDATA[%s]]></Content>
						<FuncFlag>0</FuncFlag>
						</xml>";             
			if(!empty( $keyword ))
            {
          		$msgType = "text";
                if(!$istoolong){
                    $contentStr = "弹幕已发射~";
                }else{
                    $contentStr = "太长了，弹幕君吃不下哦~";
                }
            	
            	$resultStr = sprintf($textTpl, $fromUsername, $toUsername, $time, $msgType, $contentStr);
            	echo $resultStr;
            }
            // 若微信号通过认证，解除注释/**/
            //get access_token
            /*
            $url = 'https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid=' . $appid . '&secret=' . $appsecret;
            $ch = curl_init();
            curl_setopt($ch, CURLOPT_URL, $url);
            curl_setopt($ch, CURLOPT_SSL_VERIFYPEER, FALSE); 
            curl_setopt($ch, CURLOPT_SSL_VERIFYHOST, FALSE); 
            curl_setopt($ch, CURLOPT_RETURNTRANSFER, 1);
            $output = curl_exec($ch);
            curl_close($ch);
            $jsoninfo = json_decode($output, true);
            $access_token = $jsoninfo["access_token"];
            */
            //get nikename
            /*
            $info = file_get_contents('https://api.weixin.qq.com/cgi-bin/user/info?access_token=' . $access_token . '&openid=' . $fromUsername . '&lang=zh_CN');
            $info = json_decode($info, true);
            $nickname = addcslashes($info['nickname']);
            */
            //{"errcode":48001,"errmsg":"api unauthorized hint: [HiaIHa0980vr21]"}
            //insert into db
            if(!$istoolong){
                $stmt = $GLOBALS['db']->prepare("INSERT INTO `pilipili` (`roomId`, `name`, `content`, `time`) VALUES (1, ?, ?, ?)");
                $time = time();
                // 若微信号通过认证，解除注释
                /* $stmt->bind_param('ssi', $nickname, $keyword, $time);*/
                $stmt->bind_param('ssi', $fromUsername, $keyword, $time);
                $stmt->execute();
            }
        }
    }
		
	private function checkSignature()
	{
        // you must define TOKEN by yourself
        if (!defined("TOKEN")) 
        {
            throw new Exception('TOKEN is not defined!');
        }
        
        $signature = $_GET["signature"];
        $timestamp = $_GET["timestamp"];
        $nonce = $_GET["nonce"];
        		
		$token = TOKEN;
		$tmpArr = array($token, $timestamp, $nonce);
        // use SORT_STRING rule
		sort($tmpArr, SORT_STRING);
		$tmpStr = implode( $tmpArr );
		$tmpStr = sha1( $tmpStr );
		
		if( $tmpStr == $signature )
        {
			return true;
		}else{
			return false;
		}
	}
}

?>