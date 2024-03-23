import 'dart:typed_data';

import 'package:bluetooth_low_energy/bluetooth_low_energy.dart';
import 'package:coffee_copilot/constant.dart';

UUID serviceUUID = SERVICE;
UUID characteristicUUID = CONTROLL_CHARACTERISTIC;
String deviceName = "D1533286";

Peripheral? peripheral;

Future<GattCharacteristic> getCharacteristic() async {
  await findPeripheral();
  List<GattService> services =
      await CentralManager.instance.discoverGATT(peripheral!);
  GattService service =
      services.firstWhere((service) => service.uuid == serviceUUID);
  Logger.root.info("Service found");
  List<GattCharacteristic> characteristics = service.characteristics;
  GattCharacteristic characteristic = characteristics.firstWhere(
      (characteristic) => characteristic.uuid == characteristicUUID);
  Logger.root.info("Characteristic found");
  return characteristic;
}

Future<void> setupBluetooth() async {
  await CentralManager.instance.setUp();
  Logger.root.info("CentralManager setup");
}

Future<Peripheral?> findPeripheral() async {
  if (peripheral != null) {
    return peripheral!;
  }
  await CentralManager.instance.startDiscovery();
  int count = 0;
  peripheral = (await CentralManager.instance.discovered.firstWhere((element) =>
          element.advertisement.name == deviceName || count++ > 200))
      .peripheral;
  if (peripheral == null) {
    return null;
  }
  Logger.root.info("Peripheral found");
  await CentralManager.instance.connect(peripheral!);
  Logger.root.info("Connected to peripheral");
  await CentralManager.instance.stopDiscovery();
  Logger.root.info("Discovery stopped");
  return peripheral!;
}

Future<void> sendCommand(String type) async {
  Uint8List value = DEBUG;

  switch (type) {
    case "WAKEUP":
      value = BYTES_POWER;
      break;
    case "COFFEE":
      value = COFFE_ON;
      break;
    case "DOPPIO":
      value = DOPPIO_ON;
      break;
    case "ESPRESSO":
      value = ESPRESSO_ON;
      break;
    case "ESPRESSO2":
      value = ESPRESSO2_ON;
      break;
    default:
      throw Exception("Invalid command");
  }

  GattCharacteristic characteristic = await getCharacteristic();
  await CentralManager.instance.writeCharacteristic(characteristic,
      value: value, type: GattCharacteristicWriteType.withResponse);
}
