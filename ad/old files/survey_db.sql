-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: Nov 21, 2025 at 02:13 PM
-- Server version: 10.4.32-MariaDB
-- PHP Version: 8.0.30

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `survey_db`
--

-- --------------------------------------------------------

--
-- Table structure for table `admin_accounts`
--

CREATE TABLE `admin_accounts` (
  `admin_id` int(11) NOT NULL,
  `username` varchar(50) NOT NULL,
  `password_hash` varchar(255) NOT NULL,
  `full_name` varchar(100) DEFAULT NULL,
  `created_at` timestamp NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `categories`
--

CREATE TABLE `categories` (
  `id` int(11) NOT NULL,
  `name` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `categories`
--

INSERT INTO `categories` (`id`, `name`) VALUES
(1, 'Service Quality'),
(2, 'Accessibility & Convenience'),
(3, 'Communication & Support'),
(4, 'Value for Money'),
(5, 'Overall Experience');

-- --------------------------------------------------------

--
-- Table structure for table `questions`
--

CREATE TABLE `questions` (
  `id` int(11) NOT NULL,
  `category_id` int(11) NOT NULL,
  `question_text` text NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `questions`
--

INSERT INTO `questions` (`id`, `category_id`, `question_text`) VALUES
(1, 1, 'The service met my expectations.'),
(2, 1, 'The staff were courteous and professional.'),
(3, 1, 'My concerns were addressed quickly and effectively.'),
(4, 1, 'The overall quality of the service was excellent.'),
(5, 2, 'The service was easy to access or avail.'),
(6, 2, 'Information about the service was clear and easy to find.'),
(7, 2, 'The location or online access was convenient for me.'),
(8, 2, 'The operating hours or response time were satisfactory.'),
(9, 3, 'Staff or representatives communicated clearly.'),
(10, 3, 'I received prompt updates or feedback regarding my inquiries.'),
(11, 3, 'Communication channels (email, chat, phone) were helpful.'),
(12, 3, 'I felt heard and understood during my interactions.'),
(13, 4, 'The cost of the service was reasonable for its quality.'),
(14, 4, 'I am satisfied with the overall value I received.'),
(15, 4, 'I would recommend this service to others.'),
(16, 4, 'I would continue using this service in the future.'),
(17, 5, 'I am satisfied with my overall experience.'),
(18, 5, 'The service exceeded my expectations.'),
(19, 5, 'I feel confident using this service again.'),
(20, 5, 'I would rate this organization highly based on my experience.');

-- --------------------------------------------------------

--
-- Table structure for table `responses`
--

CREATE TABLE `responses` (
  `id` int(11) NOT NULL,
  `submitted_at` timestamp NOT NULL DEFAULT current_timestamp(),
  `overall_score` decimal(4,2) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `responses`
--

INSERT INTO `responses` (`id`, `submitted_at`, `overall_score`) VALUES
(1, '2025-11-07 14:00:12', 2.90),
(2, '2025-11-07 16:33:07', 3.10),
(3, '2025-11-07 16:35:06', 3.65),
(4, '2025-11-07 16:37:43', 3.80),
(5, '2025-11-07 16:42:29', 2.90),
(6, '2025-11-07 16:44:29', 4.30),
(7, '2025-11-07 16:54:03', 2.90),
(8, '2025-11-07 16:59:36', 4.65);

-- --------------------------------------------------------

--
-- Table structure for table `response_answers`
--

CREATE TABLE `response_answers` (
  `id` int(11) NOT NULL,
  `response_id` int(11) NOT NULL,
  `question_id` int(11) NOT NULL,
  `rating_value` decimal(3,1) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `response_answers`
--

INSERT INTO `response_answers` (`id`, `response_id`, `question_id`, `rating_value`) VALUES
(1, 1, 1, 1.0),
(2, 1, 2, 3.0),
(3, 1, 3, 1.0),
(4, 1, 4, 5.0),
(5, 1, 5, 3.0),
(6, 1, 6, 3.0),
(7, 1, 7, 5.0),
(8, 1, 8, 2.0),
(9, 1, 9, 5.0),
(10, 1, 10, 2.0),
(11, 1, 11, 3.0),
(12, 1, 12, 5.0),
(13, 1, 13, 2.0),
(14, 1, 14, 3.0),
(15, 1, 15, 1.0),
(16, 1, 16, 4.0),
(17, 1, 17, 1.0),
(18, 1, 18, 2.0),
(19, 1, 19, 3.0),
(20, 1, 20, 4.0),
(21, 2, 1, 1.0),
(22, 2, 2, 2.0),
(23, 2, 3, 3.0),
(24, 2, 4, 5.0),
(25, 2, 5, 5.0),
(26, 2, 6, 4.0),
(27, 2, 7, 4.0),
(28, 2, 8, 3.0),
(29, 2, 9, 3.0),
(30, 2, 10, 2.0),
(31, 2, 11, 3.0),
(32, 2, 12, 5.0),
(33, 2, 13, 5.0),
(34, 2, 14, 2.0),
(35, 2, 15, 2.0),
(36, 2, 16, 1.0),
(37, 2, 17, 1.0),
(38, 2, 18, 2.0),
(39, 2, 19, 4.0),
(40, 2, 20, 5.0),
(41, 3, 1, 1.0),
(42, 3, 2, 2.0),
(43, 3, 3, 1.0),
(44, 3, 4, 1.0),
(45, 3, 5, 5.0),
(46, 3, 6, 5.0),
(47, 3, 7, 5.0),
(48, 3, 8, 5.0),
(49, 3, 9, 5.0),
(50, 3, 10, 5.0),
(51, 3, 11, 4.0),
(52, 3, 12, 4.0),
(53, 3, 13, 4.0),
(54, 3, 14, 4.0),
(55, 3, 15, 4.0),
(56, 3, 16, 4.0),
(57, 3, 17, 4.0),
(58, 3, 18, 3.0),
(59, 3, 19, 3.0),
(60, 3, 20, 4.0),
(61, 4, 1, 4.0),
(62, 4, 2, 4.0),
(63, 4, 3, 4.0),
(64, 4, 4, 4.0),
(65, 4, 5, 3.0),
(66, 4, 6, 3.0),
(67, 4, 7, 4.0),
(68, 4, 8, 5.0),
(69, 4, 9, 5.0),
(70, 4, 10, 4.0),
(71, 4, 11, 3.0),
(72, 4, 12, 4.0),
(73, 4, 13, 3.0),
(74, 4, 14, 4.0),
(75, 4, 15, 5.0),
(76, 4, 16, 4.0),
(77, 4, 17, 4.0),
(78, 4, 18, 4.0),
(79, 4, 19, 3.0),
(80, 4, 20, 2.0),
(81, 5, 1, 2.0),
(82, 5, 2, 3.0),
(83, 5, 3, 4.0),
(84, 5, 4, 3.0),
(85, 5, 5, 2.0),
(86, 5, 6, 2.0),
(87, 5, 7, 3.0),
(88, 5, 8, 4.0),
(89, 5, 9, 3.0),
(90, 5, 10, 2.0),
(91, 5, 11, 3.0),
(92, 5, 12, 4.0),
(93, 5, 13, 2.0),
(94, 5, 14, 3.0),
(95, 5, 15, 4.0),
(96, 5, 16, 2.0),
(97, 5, 17, 3.0),
(98, 5, 18, 2.0),
(99, 5, 19, 3.0),
(100, 5, 20, 4.0),
(101, 6, 1, 1.0),
(102, 6, 2, 4.0),
(103, 6, 3, 4.0),
(104, 6, 4, 4.0),
(105, 6, 5, 4.0),
(106, 6, 6, 5.0),
(107, 6, 7, 5.0),
(108, 6, 8, 5.0),
(109, 6, 9, 4.0),
(110, 6, 10, 4.0),
(111, 6, 11, 5.0),
(112, 6, 12, 5.0),
(113, 6, 13, 4.0),
(114, 6, 14, 5.0),
(115, 6, 15, 4.0),
(116, 6, 16, 5.0),
(117, 6, 17, 5.0),
(118, 6, 18, 4.0),
(119, 6, 19, 5.0),
(120, 6, 20, 4.0),
(121, 7, 1, 1.0),
(122, 7, 2, 2.0),
(123, 7, 3, 3.0),
(124, 7, 4, 4.0),
(125, 7, 5, 5.0),
(126, 7, 6, 4.0),
(127, 7, 7, 3.0),
(128, 7, 8, 2.0),
(129, 7, 9, 1.0),
(130, 7, 10, 2.0),
(131, 7, 11, 3.0),
(132, 7, 12, 4.0),
(133, 7, 13, 5.0),
(134, 7, 14, 4.0),
(135, 7, 15, 3.0),
(136, 7, 16, 2.0),
(137, 7, 17, 1.0),
(138, 7, 18, 2.0),
(139, 7, 19, 3.0),
(140, 7, 20, 4.0),
(141, 8, 1, 5.0),
(142, 8, 2, 5.0),
(143, 8, 3, 5.0),
(144, 8, 4, 5.0),
(145, 8, 5, 5.0),
(146, 8, 6, 4.0),
(147, 8, 7, 5.0),
(148, 8, 8, 5.0),
(149, 8, 9, 5.0),
(150, 8, 10, 5.0),
(151, 8, 11, 4.0),
(152, 8, 12, 4.0),
(153, 8, 13, 5.0),
(154, 8, 14, 5.0),
(155, 8, 15, 5.0),
(156, 8, 16, 5.0),
(157, 8, 17, 4.0),
(158, 8, 18, 4.0),
(159, 8, 19, 4.0),
(160, 8, 20, 4.0);

--
-- Indexes for dumped tables
--

--
-- Indexes for table `admin_accounts`
--
ALTER TABLE `admin_accounts`
  ADD PRIMARY KEY (`admin_id`),
  ADD UNIQUE KEY `username` (`username`);

--
-- Indexes for table `categories`
--
ALTER TABLE `categories`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `questions`
--
ALTER TABLE `questions`
  ADD PRIMARY KEY (`id`),
  ADD KEY `category_id` (`category_id`);

--
-- Indexes for table `responses`
--
ALTER TABLE `responses`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `response_answers`
--
ALTER TABLE `response_answers`
  ADD PRIMARY KEY (`id`),
  ADD KEY `response_id` (`response_id`),
  ADD KEY `question_id` (`question_id`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `admin_accounts`
--
ALTER TABLE `admin_accounts`
  MODIFY `admin_id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `categories`
--
ALTER TABLE `categories`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT for table `questions`
--
ALTER TABLE `questions`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=21;

--
-- AUTO_INCREMENT for table `responses`
--
ALTER TABLE `responses`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=10;

--
-- AUTO_INCREMENT for table `response_answers`
--
ALTER TABLE `response_answers`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=181;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `questions`
--
ALTER TABLE `questions`
  ADD CONSTRAINT `questions_ibfk_1` FOREIGN KEY (`category_id`) REFERENCES `categories` (`id`) ON DELETE CASCADE;

--
-- Constraints for table `response_answers`
--
ALTER TABLE `response_answers`
  ADD CONSTRAINT `response_answers_ibfk_1` FOREIGN KEY (`response_id`) REFERENCES `responses` (`id`) ON DELETE CASCADE,
  ADD CONSTRAINT `response_answers_ibfk_2` FOREIGN KEY (`question_id`) REFERENCES `questions` (`id`) ON DELETE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
