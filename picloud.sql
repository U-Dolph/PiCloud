-- phpMyAdmin SQL Dump
-- version 5.2.0
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: Sep 16, 2022 at 01:31 PM
-- Server version: 10.4.24-MariaDB
-- PHP Version: 7.4.29

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `picloud`
--

-- --------------------------------------------------------

--
-- Table structure for table `aspnetroleclaims`
--

CREATE TABLE `aspnetroleclaims` (
  `Id` int(11) NOT NULL,
  `RoleId` varchar(255) NOT NULL,
  `ClaimType` longtext DEFAULT NULL,
  `ClaimValue` longtext DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Table structure for table `aspnetroles`
--

CREATE TABLE `aspnetroles` (
  `Id` varchar(255) NOT NULL,
  `Name` varchar(256) DEFAULT NULL,
  `NormalizedName` varchar(256) DEFAULT NULL,
  `ConcurrencyStamp` longtext DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table `aspnetroles`
--

INSERT INTO `aspnetroles` (`Id`, `Name`, `NormalizedName`, `ConcurrencyStamp`) VALUES
('c024034e-3aad-490b-8889-7b357955baf5', 'admin', 'ADMIN', '112847a3-d6f2-4cc3-8216-9d6b28557ff3'),
('e23a380c-4948-4029-bd08-e44972e1ef60', 'user', 'USER', '9dc64ff1-aa62-4ca1-a470-8c5aae3b3b51');

-- --------------------------------------------------------

--
-- Table structure for table `aspnetuserclaims`
--

CREATE TABLE `aspnetuserclaims` (
  `Id` int(11) NOT NULL,
  `UserId` varchar(255) NOT NULL,
  `ClaimType` longtext DEFAULT NULL,
  `ClaimValue` longtext DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Table structure for table `aspnetuserlogins`
--

CREATE TABLE `aspnetuserlogins` (
  `LoginProvider` varchar(255) NOT NULL,
  `ProviderKey` varchar(255) NOT NULL,
  `ProviderDisplayName` longtext DEFAULT NULL,
  `UserId` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Table structure for table `aspnetuserroles`
--

CREATE TABLE `aspnetuserroles` (
  `UserId` varchar(255) NOT NULL,
  `RoleId` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table `aspnetuserroles`
--

INSERT INTO `aspnetuserroles` (`UserId`, `RoleId`) VALUES
('0dafc2ba-8a99-49c3-b4d3-706ff0ca887c', 'c024034e-3aad-490b-8889-7b357955baf5'),
('0dafc2ba-8a99-49c3-b4d3-706ff0ca887c', 'e23a380c-4948-4029-bd08-e44972e1ef60'),
('faaad253-f864-4f6e-8270-eb7e38c402ff', 'e23a380c-4948-4029-bd08-e44972e1ef60');

-- --------------------------------------------------------

--
-- Table structure for table `aspnetusers`
--

CREATE TABLE `aspnetusers` (
  `Id` varchar(255) NOT NULL,
  `UserName` varchar(256) DEFAULT NULL,
  `NormalizedUserName` varchar(256) DEFAULT NULL,
  `Email` varchar(256) DEFAULT NULL,
  `NormalizedEmail` varchar(256) DEFAULT NULL,
  `EmailConfirmed` tinyint(1) NOT NULL,
  `PasswordHash` longtext DEFAULT NULL,
  `SecurityStamp` longtext DEFAULT NULL,
  `ConcurrencyStamp` longtext DEFAULT NULL,
  `PhoneNumber` longtext DEFAULT NULL,
  `PhoneNumberConfirmed` tinyint(1) NOT NULL,
  `TwoFactorEnabled` tinyint(1) NOT NULL,
  `LockoutEnd` datetime(6) DEFAULT NULL,
  `LockoutEnabled` tinyint(1) NOT NULL,
  `AccessFailedCount` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table `aspnetusers`
--

INSERT INTO `aspnetusers` (`Id`, `UserName`, `NormalizedUserName`, `Email`, `NormalizedEmail`, `EmailConfirmed`, `PasswordHash`, `SecurityStamp`, `ConcurrencyStamp`, `PhoneNumber`, `PhoneNumberConfirmed`, `TwoFactorEnabled`, `LockoutEnd`, `LockoutEnabled`, `AccessFailedCount`) VALUES
('0dafc2ba-8a99-49c3-b4d3-706ff0ca887c', 'admin', 'ADMIN', 'm.tamas1012@gmail.com', 'M.TAMAS1012@GMAIL.COM', 0, 'AQAAAAEAACcQAAAAEDWL53060rkp5uxZQdj9EvkJ0uafF2FHD1FTgAKgI3xlwaOKJtRQQSc52RPqdIs1Lg==', 'EUT5EW2I3XKXXERUGQYZYD3CCTBYXIZE', '3afff8a9-cf13-46eb-8683-ccad96739688', NULL, 0, 0, NULL, 1, 0),
('faaad253-f864-4f6e-8270-eb7e38c402ff', 'user', 'USER', 'user@adf.com', 'USER@ADF.COM', 0, 'AQAAAAEAACcQAAAAEOdKaBNdvEBkjrNwMgt0lvcoSFGOJr4cP4CmOzEZwKcL3dX9rlUpfVA3KaJKoYNlBw==', 'EIL2MKMLMKNW4HXZAREQTOZKYE6QLTE6', '2cb73764-ed4c-45fb-8135-55fabf7e8b9c', NULL, 0, 0, NULL, 1, 0);

-- --------------------------------------------------------

--
-- Table structure for table `aspnetusertokens`
--

CREATE TABLE `aspnetusertokens` (
  `UserId` varchar(255) NOT NULL,
  `LoginProvider` varchar(255) NOT NULL,
  `Name` varchar(255) NOT NULL,
  `Value` longtext DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Table structure for table `games`
--

CREATE TABLE `games` (
  `Id` int(11) NOT NULL,
  `Name` longtext NOT NULL,
  `Description` longtext NOT NULL,
  `Version` longtext NOT NULL,
  `Featured` datetime(6) NOT NULL,
  `LastUpdated` datetime(6) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table `games`
--

INSERT INTO `games` (`Id`, `Name`, `Description`, `Version`, `Featured`, `LastUpdated`) VALUES
(1, 'Amazed', 'an amazing game', '1.0.2', '0001-01-01 00:00:00.000000', '2022-09-11 15:23:26.443000'),
(2, 'Dash', 'yea boi, végre kész', '0.8.3d', '2022-09-10 19:43:13.942000', '2022-09-10 19:43:13.942000');

-- --------------------------------------------------------

--
-- Table structure for table `refreshtokens`
--

CREATE TABLE `refreshtokens` (
  `Id` int(11) NOT NULL,
  `UserId` longtext NOT NULL,
  `Token` longtext NOT NULL,
  `JwtId` longtext NOT NULL,
  `IsUsed` tinyint(1) NOT NULL,
  `IsRevoked` tinyint(1) NOT NULL,
  `AddedDate` datetime(6) NOT NULL,
  `ExpiryDate` datetime(6) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table `refreshtokens`
--

INSERT INTO `refreshtokens` (`Id`, `UserId`, `Token`, `JwtId`, `IsUsed`, `IsRevoked`, `AddedDate`, `ExpiryDate`) VALUES
(69, '0dafc2ba-8a99-49c3-b4d3-706ff0ca887c', 'CRqJXAKhYMuDYGRtLZsWwL6H3iwBXeyT', '9d4c2808-cb8d-4f1f-a653-0644ee6a314f', 1, 0, '2022-09-14 17:29:52.728974', '2023-09-14 17:29:52.729015'),
(70, '0dafc2ba-8a99-49c3-b4d3-706ff0ca887c', 'njCIJTSVOBX8zio6GqzTH4vyuQTmQVdo', '4ca8a754-eec6-4ac5-ab36-28d2ec754664', 1, 0, '2022-09-14 17:32:03.306829', '2023-09-14 17:32:03.306829'),
(71, '0dafc2ba-8a99-49c3-b4d3-706ff0ca887c', 'uXxyuWAXWmCvlYsGTDs9GA2qon52cYeo', '9bef052e-14ea-4c22-8fb2-1dd665e10cbf', 0, 0, '2022-09-14 17:33:56.172373', '2023-09-14 17:33:56.172373'),
(72, '0dafc2ba-8a99-49c3-b4d3-706ff0ca887c', 'mkGrij9ZzJWOUZH3zx6PzP0LEbVIEQCw', '39b1d494-ea91-4c03-bf65-198deaba08f0', 0, 0, '2022-09-15 09:22:54.600792', '2023-09-15 09:22:54.600837'),
(73, '0dafc2ba-8a99-49c3-b4d3-706ff0ca887c', 'XNLZBfgYW74hzShENVUCOkGLCpGuMaGM', 'ae722d82-b0ca-4c9c-9f38-e287c813d66f', 0, 0, '2022-09-15 09:24:22.388137', '2023-09-15 09:24:22.388186'),
(74, '0dafc2ba-8a99-49c3-b4d3-706ff0ca887c', 'x3m3UyxjjqHaHCvEvTqkkPFre57uCk4H', 'fb366163-5894-44fd-8a00-89bc3d5595ac', 0, 0, '2022-09-15 15:37:41.862065', '2023-09-15 15:37:41.862107'),
(75, '0dafc2ba-8a99-49c3-b4d3-706ff0ca887c', '4ZU788jx4XhuVRavAyaQbSP1qancvqoW', '026bdb29-c538-4b27-ae4b-8e0c3b043a1b', 0, 0, '2022-09-15 15:38:09.057594', '2023-09-15 15:38:09.057594'),
(76, '0dafc2ba-8a99-49c3-b4d3-706ff0ca887c', 'IcXbATxbN43lpJNsVTZzY3Cbk00sRxzS', '54f48750-2edc-4127-b3ee-8e8247adf18c', 0, 0, '2022-09-15 15:49:34.291692', '2023-09-15 15:49:34.291761'),
(77, '0dafc2ba-8a99-49c3-b4d3-706ff0ca887c', 'ZbHYPmrJvpPUTSY8EuJH9WaPdNYruhdj', '161be633-a474-4554-beaa-cd7b7645c863', 0, 0, '2022-09-15 16:26:37.044046', '2023-09-15 16:26:37.044093'),
(78, 'faaad253-f864-4f6e-8270-eb7e38c402ff', 'D9L79jdm5HXdDFHohJgRLJygykRfEBp3', '5a609d62-a62a-4c67-989c-85438f1aec04', 0, 0, '2022-09-15 16:26:45.953264', '2023-09-15 16:26:45.953264'),
(79, '0dafc2ba-8a99-49c3-b4d3-706ff0ca887c', '2L5Xv1wDRG9TYoQ62FiOHC5ywhCRR99V', '7e9ecde7-6222-4160-935d-003cc0901d32', 0, 0, '2022-09-15 16:27:15.848439', '2023-09-15 16:27:15.848439'),
(80, '0dafc2ba-8a99-49c3-b4d3-706ff0ca887c', 'ghBcuWy0pQHd4rXRW7hnGim5PbCDxQN0', '0506ac1d-ce52-4dcf-8b20-cb10e8d44741', 0, 0, '2022-09-15 16:27:30.371326', '2023-09-15 16:27:30.371326'),
(81, '0dafc2ba-8a99-49c3-b4d3-706ff0ca887c', 'c6OdwZxq9zzmoNoclz5pH4Ml6QxhX1s5', '7187fcc2-b62c-41bf-ab3a-5cc4b7850e7a', 0, 0, '2022-09-15 16:33:21.220038', '2023-09-15 16:33:21.220085');

-- --------------------------------------------------------

--
-- Table structure for table `__efmigrationshistory`
--

CREATE TABLE `__efmigrationshistory` (
  `MigrationId` varchar(150) NOT NULL,
  `ProductVersion` varchar(32) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table `__efmigrationshistory`
--

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`) VALUES
('20220909175759_InitialMigration', '6.0.8'),
('20220910112256_InitMig', '6.0.8'),
('20220910144105_initialMig', '6.0.8'),
('20220910144301_InitMig', '6.0.8'),
('20220910144539_second', '6.0.8'),
('20220910144704_3', '6.0.8'),
('20220910174638_update', '6.0.8'),
('20220911130536_refresh_token', '6.0.8');

--
-- Indexes for dumped tables
--

--
-- Indexes for table `aspnetroleclaims`
--
ALTER TABLE `aspnetroleclaims`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `IX_AspNetRoleClaims_RoleId` (`RoleId`);

--
-- Indexes for table `aspnetroles`
--
ALTER TABLE `aspnetroles`
  ADD PRIMARY KEY (`Id`),
  ADD UNIQUE KEY `RoleNameIndex` (`NormalizedName`);

--
-- Indexes for table `aspnetuserclaims`
--
ALTER TABLE `aspnetuserclaims`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `IX_AspNetUserClaims_UserId` (`UserId`);

--
-- Indexes for table `aspnetuserlogins`
--
ALTER TABLE `aspnetuserlogins`
  ADD PRIMARY KEY (`LoginProvider`,`ProviderKey`),
  ADD KEY `IX_AspNetUserLogins_UserId` (`UserId`);

--
-- Indexes for table `aspnetuserroles`
--
ALTER TABLE `aspnetuserroles`
  ADD PRIMARY KEY (`UserId`,`RoleId`),
  ADD KEY `IX_AspNetUserRoles_RoleId` (`RoleId`);

--
-- Indexes for table `aspnetusers`
--
ALTER TABLE `aspnetusers`
  ADD PRIMARY KEY (`Id`),
  ADD UNIQUE KEY `UserNameIndex` (`NormalizedUserName`),
  ADD KEY `EmailIndex` (`NormalizedEmail`);

--
-- Indexes for table `aspnetusertokens`
--
ALTER TABLE `aspnetusertokens`
  ADD PRIMARY KEY (`UserId`,`LoginProvider`,`Name`);

--
-- Indexes for table `games`
--
ALTER TABLE `games`
  ADD PRIMARY KEY (`Id`);

--
-- Indexes for table `refreshtokens`
--
ALTER TABLE `refreshtokens`
  ADD PRIMARY KEY (`Id`);

--
-- Indexes for table `__efmigrationshistory`
--
ALTER TABLE `__efmigrationshistory`
  ADD PRIMARY KEY (`MigrationId`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `aspnetroleclaims`
--
ALTER TABLE `aspnetroleclaims`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `aspnetuserclaims`
--
ALTER TABLE `aspnetuserclaims`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `games`
--
ALTER TABLE `games`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT for table `refreshtokens`
--
ALTER TABLE `refreshtokens`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=82;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `aspnetroleclaims`
--
ALTER TABLE `aspnetroleclaims`
  ADD CONSTRAINT `FK_AspNetRoleClaims_AspNetRoles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `aspnetroles` (`Id`) ON DELETE CASCADE;

--
-- Constraints for table `aspnetuserclaims`
--
ALTER TABLE `aspnetuserclaims`
  ADD CONSTRAINT `FK_AspNetUserClaims_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `aspnetusers` (`Id`) ON DELETE CASCADE;

--
-- Constraints for table `aspnetuserlogins`
--
ALTER TABLE `aspnetuserlogins`
  ADD CONSTRAINT `FK_AspNetUserLogins_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `aspnetusers` (`Id`) ON DELETE CASCADE;

--
-- Constraints for table `aspnetuserroles`
--
ALTER TABLE `aspnetuserroles`
  ADD CONSTRAINT `FK_AspNetUserRoles_AspNetRoles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `aspnetroles` (`Id`) ON DELETE CASCADE,
  ADD CONSTRAINT `FK_AspNetUserRoles_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `aspnetusers` (`Id`) ON DELETE CASCADE;

--
-- Constraints for table `aspnetusertokens`
--
ALTER TABLE `aspnetusertokens`
  ADD CONSTRAINT `FK_AspNetUserTokens_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `aspnetusers` (`Id`) ON DELETE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
