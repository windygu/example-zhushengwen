CREATE TABLE `data_xp_newstype` (
  `id` bigint(20) NOT NULL auto_increment,
  `typename` varchar(20) NOT NULL,
  `url` varchar(200) NOT NULL,
  PRIMARY KEY  (`id`)
) ENGINE=MyISAM AUTO_INCREMENT=163 DEFAULT CHARSET=utf8;
CREATE TABLE `data_xp_news` (
  `id` bigint(20) NOT NULL auto_increment COMMENT '主键',
  `title` varchar(50) NOT NULL COMMENT '新闻标题',
  `type_id` bigint(20) NOT NULL COMMENT '新闻类型外键',
  `content` text character set utf8 collate utf8_unicode_ci NOT NULL COMMENT '内容',
  `source` varchar(50) default NULL COMMENT '新闻来源',
  `misc` varchar(255) default NULL COMMENT '备注',
  `pubdate` date default NULL,
  PRIMARY KEY  (`id`),
  KEY `type` (`type_id`)
) ENGINE=MyISAM AUTO_INCREMENT=29963 DEFAULT CHARSET=utf8;