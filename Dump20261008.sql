-- MySQL dump 10.13  Distrib 8.0.41, for Win64 (x86_64)
--
-- Host: localhost    Database: welkin_restaurant
-- ------------------------------------------------------
-- Server version	9.2.0

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `menu_items`
--

DROP TABLE IF EXISTS `menu_items`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `menu_items` (
  `id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(100) NOT NULL,
  `category` enum('Пасты','Супы','Горячие блюда','Напитки','Закуски','Десерты') NOT NULL,
  `price` decimal(10,2) NOT NULL,
  `is_available` tinyint(1) DEFAULT '1',
  `description` text,
  `recipe` json DEFAULT NULL COMMENT '{"product_id": quantity} - сколько продуктов уходит на блюдо',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=32 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `menu_items`
--

LOCK TABLES `menu_items` WRITE;
/*!40000 ALTER TABLE `menu_items` DISABLE KEYS */;
INSERT INTO `menu_items` VALUES (1,'Карбонара классическая','Пасты',490.00,1,'Спагетти с беконом в сливочном соусе','{\"1\": 0.15, \"3\": 0.1, \"4\": 0.05, \"12\": 0.1}'),(2,'Болоньезе','Пасты',450.00,1,'Мясной соус по-болонски','{\"1\": 0.15, \"2\": 0.12, \"6\": 0.1, \"7\": 0.05}'),(3,'Альфредо с курицей','Пасты',520.00,1,'Нежный сливочный соус с пармезаном','{\"1\": 0.15, \"3\": 0.12, \"4\": 0.05, \"13\": 0.15}'),(4,'Четыре сыра','Пасты',550.00,1,'Смесь четырёх итальянских сыров','{\"1\": 0.15, \"3\": 0.1, \"4\": 0.08}'),(5,'Паста с морепродуктами','Пасты',680.00,1,'Креветки, мидии, кальмары','{\"1\": 0.15, \"6\": 0.08, \"9\": 0.05}'),(6,'Паста с трюфелем','Пасты',890.00,1,'Ароматный трюфельный соус','{\"1\": 0.15, \"3\": 0.12, \"4\": 0.08}'),(7,'Том Ям','Супы',420.00,1,'Острый тайский суп','{\"3\": 0.08, \"5\": 0.1, \"8\": 0.02}'),(8,'Борщ украинский','Супы',350.00,1,'Классический борщ со сметаной','{\"2\": 0.1, \"6\": 0.1, \"7\": 0.05, \"10\": 0.01}'),(9,'Солянка мясная','Супы',380.00,1,'Сборная мясная солянка','{\"2\": 0.08, \"12\": 0.08, \"13\": 0.08}'),(10,'Крем-суп грибной','Супы',320.00,1,'Из лесных грибов со сливками','{\"3\": 0.08, \"7\": 0.03}'),(11,'Лапша куриная','Супы',310.00,1,'Домашняя лапша с курицей','{\"5\": 0.08, \"7\": 0.03, \"13\": 0.12}'),(12,'Тыквенный суп','Супы',280.00,1,'С тыквой и семечками','{\"3\": 0.05}'),(13,'Стейк Рибай','Горячие блюда',1200.00,1,'Мраморная говядина','{\"2\": 0.35, \"9\": 0.03, \"10\": 0.01}'),(14,'Котлета по-киевски','Горячие блюда',420.00,1,'С маслом внутри','{\"1\": 0.05, \"9\": 0.02, \"13\": 0.2}'),(15,'Лосось на гриле','Горячие блюда',890.00,1,'С овощами гриль','{\"9\": 0.02, \"10\": 0.01}'),(16,'Утиная грудка','Горячие блюда',950.00,1,'С ягодным соусом','{\"9\": 0.02}'),(17,'Ризотто с грибами','Горячие блюда',480.00,1,'Кремовое ризотто','{\"3\": 0.08, \"4\": 0.05, \"14\": 0.2}'),(18,'Цезарь с курицей','Горячие блюда',450.00,1,'Салат с курицей и соусом цезарь','{\"4\": 0.03, \"13\": 0.15}'),(19,'Капучино','Напитки',200.00,1,'Итальянский кофе с пенкой','{\"3\": 0.02, \"15\": 0.018}'),(20,'Латте','Напитки',220.00,1,'Кофе с большим количеством молока','{\"3\": 0.03, \"15\": 0.018}'),(21,'Чай чёрный','Напитки',150.00,1,'Цейлонский чай','{\"16\": 0.005}'),(22,'Сок апельсиновый','Напитки',280.00,1,'100% апельсин','{\"17\": 0.25}'),(23,'Мохито безалкогольный','Напитки',250.00,1,'С мятой и лаймом','{}'),(24,'Лимонад домашний','Напитки',210.00,1,'Из лимонов и мяты','{}'),(25,'Брускетта с томатами','Закуски',290.00,1,'Хрустящий хлеб с томатами','{\"6\": 0.08, \"9\": 0.01}'),(26,'Оливки ассорти','Закуски',180.00,1,'Смесь оливок','{}'),(27,'Сырная тарелка','Закуски',450.00,1,'3 сорта сыра','{\"4\": 0.1}'),(28,'Карпаччо из говядины','Закуски',520.00,1,'Тонко нарезанная говядина','{\"2\": 0.12, \"9\": 0.02}'),(29,'Тирамису','Десерты',380.00,1,'Классический итальянский десерт','{}'),(30,'Чизкейк','Десерты',350.00,1,'Нью-йоркский чизкейк','{}'),(31,'Панна-котта','Десерты',320.00,1,'С ягодным соусом','{\"3\": 0.08}');
/*!40000 ALTER TABLE `menu_items` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `order_items`
--

DROP TABLE IF EXISTS `order_items`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `order_items` (
  `id` int NOT NULL AUTO_INCREMENT,
  `order_id` int NOT NULL,
  `menu_item_id` int NOT NULL,
  `quantity` int NOT NULL,
  `price_at_time` decimal(10,2) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `menu_item_id` (`menu_item_id`),
  KEY `idx_order` (`order_id`),
  CONSTRAINT `order_items_ibfk_1` FOREIGN KEY (`order_id`) REFERENCES `orders` (`id`) ON DELETE CASCADE,
  CONSTRAINT `order_items_ibfk_2` FOREIGN KEY (`menu_item_id`) REFERENCES `menu_items` (`id`),
  CONSTRAINT `order_items_chk_1` CHECK ((`quantity` > 0))
) ENGINE=InnoDB AUTO_INCREMENT=261 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `order_items`
--

LOCK TABLES `order_items` WRITE;
/*!40000 ALTER TABLE `order_items` DISABLE KEYS */;
INSERT INTO `order_items` VALUES (1,1,3,1,520.00),(2,1,2,1,450.00),(3,2,3,1,520.00),(4,2,2,1,450.00),(5,2,1,1,490.00),(6,3,5,1,680.00),(7,3,1,1,490.00),(8,4,1,1,490.00),(9,5,2,1,450.00),(10,6,1,1,490.00),(11,6,3,1,520.00),(16,10,2,1,450.00),(17,11,4,1,550.00),(20,13,3,1,520.00),(21,14,2,1,450.00),(24,16,5,1,680.00),(25,17,1,1,490.00),(26,18,3,1,520.00),(27,18,2,1,450.00),(28,19,3,1,520.00),(29,19,2,1,450.00),(30,21,5,1,680.00),(31,21,1,2,490.00),(36,24,1,1,490.00),(37,25,5,1,680.00),(38,25,5,1,680.00),(45,28,2,1,450.00),(46,29,3,1,520.00),(55,31,5,1,680.00),(56,31,1,1,490.00),(57,31,6,1,890.00),(58,31,10,1,320.00),(59,31,11,1,310.00),(60,31,17,1,480.00),(61,31,16,1,950.00),(62,31,24,1,210.00),(63,31,24,1,210.00),(64,31,24,1,210.00),(65,32,5,1,680.00),(66,33,5,1,680.00),(67,34,1,3,490.00),(68,35,3,2,520.00),(69,35,2,1,450.00),(70,36,1,1,490.00),(71,36,2,1,450.00),(72,36,3,1,520.00),(73,26,3,1,520.00),(74,26,2,1,450.00),(75,26,8,1,350.00),(76,26,10,1,320.00),(77,38,3,1,520.00),(78,38,2,1,450.00),(102,57,2,1,450.00),(103,57,11,2,310.00),(104,57,15,1,890.00),(105,57,13,1,1200.00),(106,57,24,1,210.00),(107,57,22,1,280.00),(108,57,21,1,150.00),(121,72,2,1,450.00),(122,72,1,1,490.00),(123,73,3,1,520.00),(175,116,2,1,450.00),(176,116,4,1,550.00),(177,116,5,1,680.00),(178,117,3,1,520.00),(179,119,3,1,520.00),(180,119,2,1,450.00),(181,121,3,1,520.00),(182,121,2,1,450.00),(183,121,1,1,490.00),(188,123,5,1,680.00),(189,123,1,1,490.00),(190,123,2,1,450.00),(191,123,3,1,520.00),(192,124,22,10,280.00),(193,125,3,1,520.00),(194,125,2,1,450.00),(195,125,1,1,490.00),(196,127,1,1,490.00),(197,127,2,1,450.00),(198,127,3,1,520.00),(199,128,5,1,680.00),(203,135,5,1,680.00),(204,135,1,1,490.00),(205,135,2,1,450.00),(206,135,3,1,520.00),(210,136,3,1,520.00),(211,136,2,1,450.00),(212,136,1,1,490.00),(220,141,2,1,450.00),(221,142,3,1,520.00),(226,146,5,1,680.00),(227,146,1,1,490.00),(228,146,2,1,450.00),(229,146,3,1,520.00),(230,144,3,2,520.00),(231,144,2,1,450.00),(232,144,1,1,490.00),(233,144,5,1,680.00),(238,150,3,1,2080.00),(239,150,2,1,1800.00),(245,151,3,1,2080.00),(246,151,8,2,1400.00),(247,151,10,1,1280.00),(248,151,14,1,1680.00),(249,151,23,1,1000.00),(250,152,3,1,2080.00),(251,152,2,1,1800.00),(252,152,19,1,800.00),(258,159,2,1,1800.00),(259,159,11,1,1240.00),(260,160,3,1,2080.00);
/*!40000 ALTER TABLE `order_items` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `orders`
--

DROP TABLE IF EXISTS `orders`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `orders` (
  `id` int NOT NULL AUTO_INCREMENT,
  `table_id` int NOT NULL,
  `waiter_id` int NOT NULL,
  `created_at` datetime DEFAULT CURRENT_TIMESTAMP,
  `closed_at` datetime DEFAULT NULL,
  `total` decimal(10,2) DEFAULT '0.00',
  `status` enum('active','paid','cancelled') DEFAULT 'active',
  PRIMARY KEY (`id`),
  KEY `table_id` (`table_id`),
  KEY `waiter_id` (`waiter_id`),
  KEY `idx_status` (`status`),
  KEY `idx_created` (`created_at`),
  CONSTRAINT `orders_ibfk_1` FOREIGN KEY (`table_id`) REFERENCES `tables` (`id`),
  CONSTRAINT `orders_ibfk_2` FOREIGN KEY (`waiter_id`) REFERENCES `users` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=161 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `orders`
--

LOCK TABLES `orders` WRITE;
/*!40000 ALTER TABLE `orders` DISABLE KEYS */;
INSERT INTO `orders` VALUES (1,1,3,'2026-04-17 22:38:18','2026-04-17 22:38:32',970.00,'paid'),(2,1,1,'2026-04-17 22:39:38','2026-04-17 22:40:31',1460.00,'paid'),(3,2,2,'2026-04-17 22:39:58','2026-04-17 22:40:14',1170.00,'paid'),(4,1,3,'2026-04-17 23:31:00','2026-04-17 23:31:48',490.00,'paid'),(5,1,3,'2026-04-17 23:36:37','2026-04-17 23:36:43',450.00,'paid'),(6,1,3,'2026-04-17 23:39:23','2026-04-17 23:39:29',1010.00,'paid'),(7,1,3,'2026-04-17 23:43:03',NULL,0.00,'active'),(8,2,3,'2026-04-17 23:43:36',NULL,0.00,'active'),(9,2,3,'2026-04-17 23:44:23',NULL,1000.00,'cancelled'),(10,2,3,'2026-04-18 09:05:11',NULL,450.00,'paid'),(11,2,3,'2026-04-18 09:05:11',NULL,550.00,'paid'),(12,2,3,'2026-04-18 09:05:13',NULL,970.00,'cancelled'),(13,2,3,'2026-04-18 09:10:09',NULL,520.00,'paid'),(14,2,3,'2026-04-18 09:10:09',NULL,450.00,'paid'),(15,1,1,'2026-04-18 09:10:39',NULL,1170.00,'cancelled'),(16,1,1,'2026-04-18 09:11:19',NULL,680.00,'paid'),(17,1,1,'2026-04-18 09:11:19',NULL,490.00,'paid'),(18,2,2,'2026-04-18 09:14:51',NULL,970.00,'active'),(19,3,1,'2026-04-18 09:17:11',NULL,970.00,'active'),(21,5,1,'2026-04-18 09:42:23',NULL,1660.00,'active'),(23,4,3,'2026-04-18 09:45:45',NULL,1850.00,'cancelled'),(24,4,3,'2026-04-18 09:46:18',NULL,490.00,'paid'),(25,4,3,'2026-04-18 09:46:18',NULL,1360.00,'paid'),(26,2,3,'2026-04-18 09:52:32',NULL,1640.00,'active'),(27,3,1,'2026-04-18 10:05:50',NULL,970.00,'cancelled'),(28,3,1,'2026-04-18 10:06:07',NULL,450.00,'paid'),(29,3,1,'2026-04-18 10:06:07',NULL,520.00,'paid'),(30,3,1,'2026-04-18 10:06:09',NULL,6600.00,'cancelled'),(31,3,1,'2026-04-18 10:06:53',NULL,4750.00,'paid'),(32,3,1,'2026-04-18 10:06:53',NULL,680.00,'paid'),(33,3,1,'2026-04-18 10:06:53',NULL,680.00,'paid'),(34,3,1,'2026-04-18 10:06:53',NULL,1470.00,'paid'),(35,3,3,'2026-04-18 10:08:03','2026-04-18 10:18:01',1490.00,'paid'),(36,4,1,'2026-04-18 10:12:29','2026-04-18 10:14:18',1460.00,'paid'),(38,1,3,'2026-04-18 10:35:13','2026-04-18 10:38:15',970.00,'paid'),(57,1,3,'2026-04-18 12:05:36','2026-04-21 21:04:45',3800.00,'paid'),(70,2,2,'2026-04-18 12:41:38',NULL,2200.00,'cancelled'),(71,3,3,'2026-04-18 12:43:42',NULL,1460.00,'cancelled'),(72,3,3,'2026-04-18 12:44:02',NULL,940.00,'paid'),(73,3,3,'2026-04-18 12:44:02',NULL,520.00,'paid'),(116,2,3,'2026-04-20 19:02:12',NULL,1680.00,'paid'),(117,2,3,'2026-04-20 19:02:12',NULL,520.00,'paid'),(119,2,2,'2026-04-20 19:54:25','2026-04-20 20:49:19',970.00,'paid'),(121,3,2,'2026-04-20 19:54:55','2026-04-20 20:53:03',1460.00,'paid'),(122,4,1,'2026-04-20 19:59:24',NULL,2140.00,'cancelled'),(123,5,2,'2026-04-20 20:11:31','2026-04-21 21:04:28',2140.00,'paid'),(124,2,3,'2026-04-20 20:49:39','2026-04-20 20:49:52',2800.00,'paid'),(125,3,2,'2026-04-21 19:30:31','2026-04-21 21:04:23',1460.00,'paid'),(127,4,3,'2026-04-21 21:04:40',NULL,1460.00,'paid'),(128,4,3,'2026-04-21 21:04:40',NULL,680.00,'paid'),(135,2,3,'2026-04-21 21:53:35','2026-04-21 21:53:39',2140.00,'paid'),(136,1,1,'2026-04-22 20:17:05','2026-04-22 20:19:38',1460.00,'paid'),(140,1,3,'2026-04-22 20:38:22',NULL,970.00,'cancelled'),(141,1,3,'2026-04-22 20:40:49',NULL,450.00,'paid'),(142,1,3,'2026-04-22 20:40:49',NULL,520.00,'paid'),(144,1,1,'2026-04-22 20:50:40','2026-06-03 16:12:14',2660.00,'paid'),(146,2,2,'2026-04-22 20:53:10',NULL,2140.00,'active'),(150,3,1,'2026-07-02 11:00:42','2026-07-02 12:11:38',3880.00,'paid'),(151,4,1,'2026-07-02 11:32:08','2026-07-02 11:34:05',8840.00,'paid'),(152,4,2,'2026-07-02 11:34:24',NULL,4680.00,'active'),(158,3,1,'2026-07-02 12:12:08',NULL,5120.00,'cancelled'),(159,3,1,'2026-07-02 12:13:17',NULL,3040.00,'paid'),(160,3,1,'2026-07-02 12:13:17',NULL,2080.00,'paid');
/*!40000 ALTER TABLE `orders` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `payments`
--

DROP TABLE IF EXISTS `payments`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `payments` (
  `id` int NOT NULL AUTO_INCREMENT,
  `order_id` int NOT NULL,
  `amount` decimal(10,2) NOT NULL,
  `payment_type` enum('cash','card','online') NOT NULL,
  `waiter_id` int NOT NULL,
  `created_at` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `order_id` (`order_id`),
  KEY `waiter_id` (`waiter_id`),
  KEY `idx_created` (`created_at`),
  CONSTRAINT `payments_ibfk_1` FOREIGN KEY (`order_id`) REFERENCES `orders` (`id`),
  CONSTRAINT `payments_ibfk_2` FOREIGN KEY (`waiter_id`) REFERENCES `users` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=66 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `payments`
--

LOCK TABLES `payments` WRITE;
/*!40000 ALTER TABLE `payments` DISABLE KEYS */;
INSERT INTO `payments` VALUES (1,1,970.00,'card',3,'2026-04-17 22:38:32'),(2,3,1170.00,'cash',2,'2026-04-17 22:40:14'),(3,2,1460.00,'cash',3,'2026-04-17 22:40:31'),(4,4,490.00,'cash',3,'2026-04-17 23:31:48'),(5,5,450.00,'card',3,'2026-04-17 23:36:43'),(6,6,1010.00,'cash',3,'2026-04-17 23:39:29'),(7,10,450.00,'cash',3,'2026-04-18 09:05:11'),(8,11,550.00,'cash',3,'2026-04-18 09:05:11'),(9,13,520.00,'cash',3,'2026-04-18 09:10:09'),(10,14,450.00,'cash',3,'2026-04-18 09:10:09'),(11,16,680.00,'cash',1,'2026-04-18 09:11:19'),(12,17,490.00,'cash',1,'2026-04-18 09:11:19'),(13,24,490.00,'cash',3,'2026-04-18 09:46:18'),(14,25,1360.00,'cash',3,'2026-04-18 09:46:18'),(15,28,450.00,'cash',1,'2026-04-18 10:06:07'),(16,29,520.00,'cash',1,'2026-04-18 10:06:07'),(17,31,4750.00,'cash',1,'2026-04-18 10:06:53'),(18,32,680.00,'cash',1,'2026-04-18 10:06:53'),(19,33,680.00,'cash',1,'2026-04-18 10:06:53'),(20,34,1470.00,'cash',1,'2026-04-18 10:06:53'),(21,36,1460.00,'cash',1,'2026-04-18 10:14:18'),(22,35,1490.00,'online',3,'2026-04-18 10:18:01'),(23,38,970.00,'card',3,'2026-04-18 10:38:15'),(27,72,940.00,'cash',3,'2026-04-18 12:44:02'),(28,73,520.00,'cash',3,'2026-04-18 12:44:02'),(45,116,1680.00,'cash',3,'2026-04-20 19:02:12'),(46,117,520.00,'cash',3,'2026-04-20 19:02:12'),(47,119,970.00,'cash',3,'2026-04-20 20:49:19'),(48,124,2800.00,'cash',3,'2026-04-20 20:49:52'),(49,121,1460.00,'card',3,'2026-04-20 20:53:03'),(50,125,1460.00,'cash',3,'2026-04-21 21:04:23'),(51,123,2140.00,'cash',3,'2026-04-21 21:04:28'),(52,127,1460.00,'cash',3,'2026-04-21 21:04:40'),(53,128,680.00,'cash',3,'2026-04-21 21:04:40'),(54,57,3800.00,'card',3,'2026-04-21 21:04:45'),(56,135,2140.00,'cash',3,'2026-04-21 21:53:39'),(58,136,1460.00,'cash',3,'2026-04-22 20:19:38'),(59,141,450.00,'cash',3,'2026-04-22 20:40:49'),(60,142,520.00,'cash',3,'2026-04-22 20:40:49'),(61,144,2660.00,'cash',3,'2026-06-03 16:12:14'),(62,151,8840.00,'cash',1,'2026-07-02 11:34:05'),(63,150,3880.00,'card',1,'2026-07-02 12:11:38'),(64,159,3040.00,'cash',1,'2026-07-02 12:13:17'),(65,160,2080.00,'cash',1,'2026-07-02 12:13:17');
/*!40000 ALTER TABLE `payments` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `product_write_offs`
--

DROP TABLE IF EXISTS `product_write_offs`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `product_write_offs` (
  `id` int NOT NULL AUTO_INCREMENT,
  `product_id` int NOT NULL,
  `order_id` int NOT NULL,
  `quantity` decimal(10,2) NOT NULL,
  `written_off_at` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `product_id` (`product_id`),
  KEY `order_id` (`order_id`),
  CONSTRAINT `product_write_offs_ibfk_1` FOREIGN KEY (`product_id`) REFERENCES `products` (`id`),
  CONSTRAINT `product_write_offs_ibfk_2` FOREIGN KEY (`order_id`) REFERENCES `orders` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `product_write_offs`
--

LOCK TABLES `product_write_offs` WRITE;
/*!40000 ALTER TABLE `product_write_offs` DISABLE KEYS */;
/*!40000 ALTER TABLE `product_write_offs` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `products`
--

DROP TABLE IF EXISTS `products`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `products` (
  `id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(100) NOT NULL,
  `unit` varchar(20) NOT NULL COMMENT 'единица измерения: кг, шт, л',
  `quantity` decimal(10,2) NOT NULL DEFAULT '0.00' COMMENT 'текущий остаток',
  `min_quantity` decimal(10,2) DEFAULT '0.00' COMMENT 'минимальный остаток для уведомления',
  `updated_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=19 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `products`
--

LOCK TABLES `products` WRITE;
/*!40000 ALTER TABLE `products` DISABLE KEYS */;
INSERT INTO `products` VALUES (1,'Мука пшеничная','кг',20.40,5.00,'2026-07-02 09:12:31'),(2,'Говядина вырезка','кг',13.28,3.00,'2026-07-02 09:12:31'),(3,'Сливки 33%','л',7.86,2.00,'2026-07-02 09:12:31'),(4,'Пармезан','кг',4.15,1.00,'2026-07-02 09:12:31'),(5,'Лапша рисовая','кг',7.92,2.00,'2026-07-02 09:12:31'),(6,'Томаты свежие','кг',10.34,3.00,'2026-07-02 09:12:31'),(7,'Лук репчатый','кг',19.16,5.00,'2026-07-02 09:12:31'),(8,'Чеснок','кг',3.00,1.00,'2026-04-17 18:09:43'),(9,'Масло оливковое','л',14.86,4.00,'2026-07-02 08:34:00'),(10,'Соль морская','кг',4.96,1.00,'2026-07-02 08:34:00'),(11,'Перец чёрный','кг',2.00,0.50,'2026-04-17 18:09:43'),(12,'Бекон','кг',11.60,2.00,'2026-04-22 17:53:20'),(13,'Куриное филе','кг',9.53,3.00,'2026-07-02 09:12:31'),(14,'Рис','кг',10.00,2.00,'2026-04-17 18:09:43'),(15,'Кофе зерновой','кг',5.98,2.00,'2026-07-02 08:34:31'),(16,'Чай листовой','кг',4.00,1.00,'2026-04-17 18:09:43'),(17,'Сок апельсиновый','л',17.50,5.00,'2026-04-20 17:49:49'),(18,'Вода минеральная','л',60.00,10.00,'2026-04-17 20:30:53');
/*!40000 ALTER TABLE `products` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `shifts`
--

DROP TABLE IF EXISTS `shifts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `shifts` (
  `id` int NOT NULL AUTO_INCREMENT,
  `opened_at` datetime DEFAULT CURRENT_TIMESTAMP,
  `closed_at` datetime DEFAULT NULL,
  `opened_by_admin_id` int NOT NULL,
  `closed_by_admin_id` int DEFAULT NULL,
  `total_cash` decimal(10,2) DEFAULT '0.00',
  `total_card` decimal(10,2) DEFAULT '0.00',
  PRIMARY KEY (`id`),
  KEY `opened_by_admin_id` (`opened_by_admin_id`),
  KEY `closed_by_admin_id` (`closed_by_admin_id`),
  CONSTRAINT `shifts_ibfk_1` FOREIGN KEY (`opened_by_admin_id`) REFERENCES `users` (`id`),
  CONSTRAINT `shifts_ibfk_2` FOREIGN KEY (`closed_by_admin_id`) REFERENCES `users` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=20 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `shifts`
--

LOCK TABLES `shifts` WRITE;
/*!40000 ALTER TABLE `shifts` DISABLE KEYS */;
INSERT INTO `shifts` VALUES (1,'2026-04-17 23:33:47','2026-04-17 23:33:49',3,3,0.00,0.00),(2,'2026-04-17 23:33:49','2026-04-17 23:35:38',3,3,0.00,0.00),(3,'2026-04-17 23:35:38','2026-04-17 23:36:29',3,3,0.00,0.00),(4,'2026-04-17 23:36:29','2026-04-17 23:36:45',3,3,0.00,0.00),(5,'2026-04-17 23:36:45','2026-04-17 23:39:19',3,3,0.00,0.00),(6,'2026-04-17 23:39:19','2026-04-17 23:39:31',3,3,0.00,0.00),(7,'2026-04-17 23:39:31','2026-04-17 23:43:39',3,3,0.00,0.00),(8,'2026-04-17 23:43:39','2026-04-18 09:10:12',3,3,0.00,0.00),(9,'2026-04-18 09:10:12','2026-04-18 09:10:16',3,3,0.00,0.00),(10,'2026-04-18 09:10:16','2026-04-18 09:46:24',3,3,0.00,0.00),(11,'2026-04-18 09:46:24','2026-04-18 10:18:04',3,3,0.00,0.00),(12,'2026-04-18 10:18:04','2026-04-18 10:38:22',3,3,0.00,0.00),(13,'2026-04-18 10:38:22','2026-04-21 21:04:53',3,3,0.00,0.00),(14,'2026-04-21 21:04:53','2026-04-21 21:14:17',3,3,0.00,0.00),(15,'2026-04-21 21:14:17','2026-04-21 21:49:53',3,3,0.00,0.00),(16,'2026-04-21 21:49:53','2026-04-21 21:52:36',3,3,0.00,0.00),(17,'2026-04-21 21:52:36','2026-04-21 21:53:47',3,3,0.00,0.00),(18,'2026-04-21 21:53:47','2026-04-22 20:19:43',3,3,0.00,0.00),(19,'2026-04-22 20:19:43',NULL,3,NULL,0.00,0.00);
/*!40000 ALTER TABLE `shifts` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `tables`
--

DROP TABLE IF EXISTS `tables`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `tables` (
  `id` int NOT NULL AUTO_INCREMENT,
  `number` int NOT NULL,
  `status` enum('free','opened','reserved') DEFAULT 'free',
  `current_waiter_id` int DEFAULT NULL,
  `current_order_id` int DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `number` (`number`),
  KEY `current_waiter_id` (`current_waiter_id`),
  CONSTRAINT `tables_ibfk_1` FOREIGN KEY (`current_waiter_id`) REFERENCES `users` (`id`) ON DELETE SET NULL
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `tables`
--

LOCK TABLES `tables` WRITE;
/*!40000 ALTER TABLE `tables` DISABLE KEYS */;
INSERT INTO `tables` VALUES (1,1,'free',NULL,NULL),(2,2,'opened',2,146),(3,3,'free',NULL,NULL),(4,4,'opened',2,152),(5,5,'free',NULL,NULL),(6,6,'free',NULL,NULL);
/*!40000 ALTER TABLE `tables` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `users`
--

DROP TABLE IF EXISTS `users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `users` (
  `id` int NOT NULL AUTO_INCREMENT,
  `login` varchar(50) NOT NULL,
  `password_hash` varchar(255) NOT NULL,
  `salt` varchar(50) NOT NULL,
  `role` enum('admin','waiter') NOT NULL,
  `full_name` varchar(100) NOT NULL,
  `created_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  UNIQUE KEY `login` (`login`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `users`
--

LOCK TABLES `users` WRITE;
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
INSERT INTO `users` VALUES (1,'waiter1','27c0ce316775465420e428f8f8c16fe706cac8e11b29984277d30d3cc8fc2d88','salt1','waiter','Иван Петров','2026-04-17 18:09:43'),(2,'waiter2','0b714a494d2adfb9012d864e4a7ab63fa0311da030d13ecb5aba74edb6c7f613','salt2','waiter','Мария Сидорова','2026-04-17 18:09:43'),(3,'admin','94919a94972425c7c4cc42da99ebcfdb1469e0d213a27123bfb3ec525d189b34','admin_salt','admin','Алексей Иванов','2026-04-17 18:09:43');
/*!40000 ALTER TABLE `users` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-10-08 21:15:16
