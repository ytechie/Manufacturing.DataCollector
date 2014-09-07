Manufacturing.DataCollector
=======================

A framework for collecting data from a manufacturing facility.

Data is collected in 2 ways:

1. A self-hosted WebAPI in the service able to receive data **pushed to it**.
2. A basic scheduler that uses adapters to **pull** data from virtually anything.

There are a few [simulation datasources](https://github.com/ytechie/Manufacturing.DataCollector/tree/master/Datasources/Simulation) for testing purposes. For example, the *RandomDatasource* generates random data at configurable intervals. The *CPUDatsource* reads CPU usage on a regular interval so that you can collect real-world data from a "sensor" in your computer.

By default, data is stored in a local MS Message Queue, which is available on any Windows machine, and can be configured for high availability with clustering.

## Background

Many of the technologies used in today’s manufacturing facility are based on technologies that were defined decades ago. For example, the Modbus protocol, a de facto standard for manufacturing, was originally published in 1979, and remains relatively unchanged today.
 
Manufacturing environments have a wide variety of data sources from varying layers. The high-level logical layers are listed below along with common implementations.

1. Physical Layer
	* 0-10v
	* 4-20mA
	* Pulse
2. Link Layer
	* RS-422 Serial
	* RS-485 Serial
3. Transport Layer
	* Modbus
	* OPC
4. Application Layer
	* Modbus TCP
	* XML

Traditionally, ISV's have built software that is installed within a manufacturing facility. This software services as the central point of collection, storage, and all data processing. Historically, nearly all of this data was available locally only.

This manufacturing framework architecture is designed to insert pluggable adapters that translate protocols from the application layer to a common format. Outside the scope of this architecture is a strategy for converting from lower layers to the application layer. There are numerous industry standard devices commonly available to accomplish this task.

Custom adapters can be created for **anything**. Simply implement `IDatasource`. You could create adapters for sensors, historians, or any other source of data.

## Prerequisites

* Since data is stored in a local MSMQ by default, you'll need to have this Windows feature installed. Fortunately, it's available in all editions of Windows. Turn it on in *Add/Remove Programs* under *Windows Features*.

![Turn on MSMQ Windows Feature](Documentation/msmq-windows-feature.gif)

# License

Microsoft Developer Experience & Evangelism

Copyright (c) Microsoft Corporation. All rights reserved.

THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.

The example companies, organizations, products, domain names, e-mail addresses, logos, people, places, and events depicted herein are fictitious. No association with any real company, organization, product, domain name, email address, logo, person, places, or events is intended or should be inferred.
