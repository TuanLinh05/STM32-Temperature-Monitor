# 🌡️ STM32 Temperature Monitor

![MCU](https://img.shields.io/badge/MCU-STM32F103C8T6-03234B?logo=stmicroelectronics&logoColor=white)
![PCB](https://img.shields.io/badge/PCB-Altium%20Designer-A5915F)
![GUI](https://img.shields.io/badge/GUI-C%23%20WinForms-512BD4?logo=dotnet&logoColor=white)
![Language](https://img.shields.io/badge/Language-C%20%7C%20C%23-555555)

<a id="english"></a>**🇬🇧 English** · [🇻🇳 Tiếng Việt](#tieng-viet)

A complete temperature measurement system: a **thermistor-based sensor board** (custom PCB), **STM32F103 firmware** that measures the signal period and converts it to temperature, and a **C# WinForms desktop app** for real-time monitoring and logging.

Course project (BTL ĐLCN) at HCM University of Technology.

---

## ✨ Features

- **Period-based measurement:** the sensor circuit turns the thermistor resistance into a square wave; TIM2 **input capture** on PA0 (1 MHz timer clock) measures its period in µs.
- **Temperature conversion:** `1/T = 1/298.15 − period / 5569417` (Kelvin → °C).
- **Two-stage filtering:** a **median filter (21 samples)** removes spikes, then an **EMA (α = 0.1)** smooths the output.
- **Linear calibration:** `T = 1.104 × T_filtered − 1.7`.
- **UART streaming:** sends `TEMP:xx.xx` every 500 ms at 115200 baud.
- **Desktop monitor:** live chart (ScottPlot), current / min / max / average values, sample counter and **CSV export**.

## 🏗️ System overview

```text
┌──────────────┐  square wave  ┌──────────────────────┐  UART 115200  ┌──────────────────────┐
│ Thermistor + │ ────────────▶ │ STM32F103            │ ────────────▶ │ TemperatureMonitor   │
│ oscillator   │   (period)    │ TIM2 input capture   │ "TEMP:25.43"  │ (C# WinForms)        │
│ (custom PCB) │               │ median + EMA + calib │               │ chart · stats · CSV  │
└──────────────┘               └──────────────────────┘               └──────────────────────┘
```

## 📂 Project structure

```text
BTLĐLCN/
├── Hardware/BTL_DLCN/        # Altium Designer project: schematic + PCB
├── MCU/BTL/                  # STM32CubeIDE firmware (main.c: capture, filter, UART)
└── GUI/TemperatureMonitor/   # C# WinForms monitoring app (.sln)
```

## 🚀 Getting started

**Firmware**
1. Open `BTLĐLCN/MCU/BTL` in **STM32CubeIDE**, build and flash.
2. Connect USART1 (PA9 TX / PA10 RX) to the PC through a USB-TTL adapter.

**GUI**
1. Open `BTLĐLCN/GUI/TemperatureMonitor/TemperatureMonitor.sln` in **Visual Studio**.
2. Run the app, choose the COM port and baud rate (115200), then press **Connect**.
3. Use **Export CSV** to save the recorded data.

**Hardware**
- Open `BTLĐLCN/Hardware/BTL_DLCN/BTL_DLCN.PrjPcb` in **Altium Designer**.

---

<a id="tieng-viet"></a>

## 🇻🇳 Tiếng Việt

[🇬🇧 English](#english) · **🇻🇳 Tiếng Việt**

Hệ thống đo nhiệt độ hoàn chỉnh gồm **mạch cảm biến dùng thermistor** (PCB tự thiết kế), **firmware STM32F103** đo chu kỳ tín hiệu rồi quy đổi ra nhiệt độ, và **phần mềm C# WinForms** giám sát, ghi dữ liệu theo thời gian thực.

Bài tập lớn (BTL ĐLCN), Trường Đại học Bách khoa – ĐHQG TP.HCM.

### ✨ Tính năng

- **Đo theo chu kỳ:** mạch cảm biến biến điện trở thermistor thành xung vuông. TIM2 **input capture** trên PA0 (xung nhịp timer 1 MHz) đo chu kỳ tính bằng µs.
- **Quy đổi nhiệt độ:** `1/T = 1/298.15 − period / 5569417` (Kelvin → °C).
- **Lọc 2 tầng:** **lọc trung vị 21 mẫu** loại bỏ gai nhiễu, sau đó **EMA (α = 0.1)** làm mượt.
- **Hiệu chuẩn tuyến tính:** `T = 1.104 × T_lọc − 1.7`.
- **Truyền UART:** gửi `TEMP:xx.xx` mỗi 500 ms, baud 115200.
- **Phần mềm giám sát:** biểu đồ trực tiếp (ScottPlot), giá trị hiện tại / nhỏ nhất / lớn nhất / trung bình, số mẫu và **xuất CSV**.

Sơ đồ hệ thống và cấu trúc thư mục: xem phần tiếng Anh ở trên.

### 🚀 Hướng dẫn sử dụng

**Firmware**
1. Mở `BTLĐLCN/MCU/BTL` bằng **STM32CubeIDE**, build và nạp.
2. Nối USART1 (PA9 TX / PA10 RX) với máy tính qua USB-TTL.

**Phần mềm**
1. Mở `BTLĐLCN/GUI/TemperatureMonitor/TemperatureMonitor.sln` bằng **Visual Studio**.
2. Chạy chương trình, chọn cổng COM, baud 115200 rồi bấm **Connect**.
3. Bấm **Export CSV** để lưu dữ liệu.

**Phần cứng**
- Mở `BTLĐLCN/Hardware/BTL_DLCN/BTL_DLCN.PrjPcb` bằng **Altium Designer**.

---

<p align="center">Made by <a href="https://github.com/TuanLinh05">Vu Tuan Linh</a> · HCMUT</p>
