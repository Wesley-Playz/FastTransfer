# FastTransfer

> A high-performance Windows file transfer utility with context menu integration and real-time speed monitoring

FastTransfer is a lightweight Windows application that provides fast, reliable file and folder transfers with real-time progress monitoring, speed tracking in both bytes and bits, and intelligent conflict resolution.

## Features

### High-Performance Transfers
- **Fast Copy/Move Operations** - Optimized transfer speeds using buffered streams (81KB buffer) and multi-threaded Robocopy
- **Real-Time Speed Monitoring** - Live transfer speed display in both **bytes/s (MB/s)** and **bits/s (Mb/s)** for both files and folders
- **Progress Tracking** - Visual progress bar with percentage, transferred size, and total size for all transfer types
- **Transfer Timing** - Automatic timing of operations with elapsed time display
- **Multi-Threading** - Robocopy uses 8 concurrent threads for folder operations
- **File Count Tracking** - Shows files transferred vs total files during folder operations
- **Parallel Size Calculation** - Folder size calculated in background while transfer is in progress for instant start

### Smart Integration
- **Windows Explorer Context Menu** - Right-click on any file or folder to transfer
- **Background Context Menu** - Right-click in empty space within folders
- **Automatic Destination Detection** - Picks up open Explorer windows as destinations
- **Multi-Transfer Support** - Start multiple transfers simultaneously in separate windows

###  Intelligent Conflict Handling
- **File Conflict Dialog** - Handle existing files with multiple options:
  - **Replace** - Overwrite the existing file
  - **Skip** - Keep the existing file and skip this one
  - **Cancel** - Stop the operation
- **Apply to All** - Option to apply your choice to all subsequent conflicts
- **Folder Merge Support** - Smart merging of folders with conflict resolution
- **Pause During Dialogs** - Transfer timer pauses while you make decisions

### User Experience
- **Clean, Modern UI** - Simple and intuitive Windows Forms interface
- **Detailed Status Updates** - Real-time feedback showing:
  - Current operation status
  - Progress percentage
  - Transfer speed in MB/s and Mb/s
  - Bytes transferred / Total bytes
- **Graceful Cancellation** - Cancel transfers at any time with proper cleanup
- **Auto-Close on Completion** - Progress window closes automatically after 2 seconds
- **Path Truncation** - Long paths are intelligently shortened for display

## Requirements

- **Operating System:** Windows 10/11 (64-bit)
- **.NET Runtime:** .NET 8 Desktop Runtime
- **Permissions:** Administrator rights for installation (context menu registration)
- **Tools:** Robocopy (built into Windows)

## Installation

### Option 1: Installer (Recommended)

1. Download `FastTransferSetup.exe` from the [Latest Build](../../releases/tag/latest-build)
2. Run the installer as Administrator
3. Follow the installation wizard
4. The installer will:
   - Install FastTransfer to `C:\Program Files\FastTransfer`
   - Register Windows context menu entries
   - Check for .NET 8 Desktop Runtime (prompts if missing)
   - Add uninstall entry to Windows Settings

### Option 2: Build from Source with NSIS

```bash
# Prerequisites: .NET 8 SDK, NSIS 3.10+

# Build the application
dotnet build FastTransfer/FastTransfer.csproj -c Release

# Create installer
makensis FastTransfer.nsi

# Output: FastTransferSetup.exe
```

### Option 3: Manual Installation

If you prefer to install manually without an installer:

1. Build the application:
   ```bash
   dotnet build FastTransfer/FastTransfer.csproj -c Release
   ```

2. Copy the contents of `FastTransfer\bin\Release\net8.0-windows\` to your desired installation folder (e.g., `C:\Program Files\FastTransfer`)

3. Add registry entries manually:

   **For files** (`HKEY_CLASSES_ROOT\*\shell\FastTransfer`):
   ```reg
   Windows Registry Editor Version 5.00

   [HKEY_CLASSES_ROOT\*\shell\FastTransfer]
   @="Fast Transfer"
   "Icon"="C:\\Program Files\\FastTransfer\\FastTransfer.exe"

   [HKEY_CLASSES_ROOT\*\shell\FastTransfer\command]
   @="\"C:\\Program Files\\FastTransfer\\FastTransfer.exe\" \"%1\""
   ```

   **For folders** (`HKEY_CLASSES_ROOT\Directory\shell\FastTransfer`):
   ```reg
   Windows Registry Editor Version 5.00

   [HKEY_CLASSES_ROOT\Directory\shell\FastTransfer]
   @="Fast Transfer"
   "Icon"="C:\\Program Files\\FastTransfer\\FastTransfer.exe"

   [HKEY_CLASSES_ROOT\Directory\shell\FastTransfer\command]
   @="\"C:\\Program Files\\FastTransfer\\FastTransfer.exe\" \"%1\""

   [HKEY_CLASSES_ROOT\Directory\Background\shell\FastTransfer]
   @="Fast Transfer"
   "Icon"="C:\\Program Files\\FastTransfer\\FastTransfer.exe"

   [HKEY_CLASSES_ROOT\Directory\Background\shell\FastTransfer\command]
   @="\"C:\\Program Files\\FastTransfer\\FastTransfer.exe\" \"%V\""
   ```

   Save these as `.reg` files and run them as administrator, or use `regedit` to add them manually.

## Usage

### Basic Transfer

1. **Open destination folders** in one or more File Explorer windows
2. **Right-click** on any file or folder you want to transfer
3. Select **"FastTransfer"** from the context menu
4. **Choose operation:** Select "Copy" or "Move" using radio buttons
5. **Select destination:** Pick from the dropdown list of open folders
6. Click **"Start"** to begin the transfer

### Transfer Progress Window

Each transfer opens its own window showing:
- **Source and Destination** paths (truncated for readability)
- **Operation Type** (Copy or Move)
- **Progress Bar** with visual feedback and percentage
  - Starts with marquee animation, then switches to percentage once total size is known
- **Status Line** showing:
  - Current operation status
  - Progress: `45.2 MB / 100 MB (45%)` or `45.2 MB transferred` (if size still calculating)
  - Speed: `5.5 MB/s (44.0 Mb/s)` - Updated every 0.5 seconds
  - For folders: File count `25 / 100 files` (or `25 files transferred` if total unknown)
- **Elapsed Time** upon completion
- **Cancel/Close** button

### File Conflicts

When a file already exists at the destination:
- **Conflict Dialog** appears with file details
- Choose your action: Replace, Skip, or Cancel
- Check **"Apply to all"** to use the same action for all conflicts
- Timer pauses during your decision - won't count against transfer time

### Folder Conflicts

When a folder already exists:
- Prompted to **Merge** (replace conflicting files) or **Skip**
- Individual file conflicts within merged folders still show the conflict dialog
- Can apply decisions to all files in the merge

### Multiple Simultaneous Transfers

- Main window stays open after clicking "Start"
- Each transfer runs in its own progress window
- Start as many transfers as you need
- All transfers are independent - cancelling one doesn't affect others
- Main window is ready for the next transfer immediately

### Copy vs Move

- **Copy**: 
  - Creates duplicate at destination
  - Keeps original file/folder
  - Safe option for important data
  
- **Move**: 
  - Transfers to destination
  - Removes original after successful transfer
  - For files: Copies then deletes source
  - For folders: Uses robocopy `/MOVE` flag for efficiency

## Building from Source

### Prerequisites
- Visual Studio 2022 or later (or VS Code with C# extension)
- .NET 8 SDK
- Windows SDK (for COM references to Shell32)

### Build Steps

```bash
# Clone the repository
git clone https://github.com/yourusername/FastTransfer.git
cd FastTransfer

# Restore dependencies
dotnet restore FastTransfer/FastTransfer.csproj

# Build in Release mode
dotnet build FastTransfer/FastTransfer.csproj --configuration Release

# Output location:
# FastTransfer/bin/Release/net8.0-windows/FastTransfer.exe
```

### Project Structure
- `FastTransfer/`
  - `Program.cs` - Entry point
  - `MainForm.cs` - Source selection and destination picker
  - `TransferProgressForm.cs` - Transfer execution with progress monitoring
  - `FileConflictDialog.cs` - Conflict resolution UI
  - `Shell32Helper.cs` - Explorer window enumeration via COM

## Performance

FastTransfer is optimized for speed:
- **File Transfers:** 81KB buffer size for optimal disk I/O performance
- **Folder Transfers:** Robocopy with 8 concurrent threads (`/MT:8`)
- **Speed Calculations:** Updated every 500ms for smooth, accurate display
- **Memory Efficient:** Streaming architecture prevents memory overflow on large files
- **Low Retry Overhead:** `/R:0 /W:0` flags on Robocopy for faster error handling
- **Parallel Size Calculation:** Folder size enumerated in background during transfer for instant start
- **Progressive Updates:** File count and size totals update every 100 files during enumeration

### Speed Display
- **Bytes:** Shows in B/s, KB/s, MB/s, or GB/s (auto-scaling)
- **Bits:** Shows in b/s, Kb/s, Mb/s, or Gb/s (useful for network context)
- Both displayed simultaneously: `5.5 MB/s (44.0 Mb/s)`
- **Available for both files and folders** with real-time updates

## Technical Details

- **Language:** C# 12.0
- **Framework:** .NET 8.0 Windows Forms
- **Build System:** MSBuild / .NET CLI
- **Installer:** NSIS 3.10
- **Architecture:** 64-bit only
- **COM References:** Shell32, SHDocVw (for Explorer integration)

## Known Limitations

- Windows-only (uses Windows-specific APIs and Robocopy)
- Requires .NET 8 Desktop Runtime
- 64-bit Windows only
- Context menu integration requires administrative installation
- Network transfers depend on network speed (Robocopy respects SMB protocol)
- Progress percentage for folders appears once size calculation completes (shown instantly for small folders)

## Roadmap

Potential future enhancements:
- [ ] Pause/Resume functionality for transfers
- [ ] Detailed transfer logs and history
- [ ] File verification (checksum) option after transfer
- [ ] Custom buffer size configuration
- [ ] Folder transfer progress with file-level detail
- [ ] Dark mode UI theme
- [ ] Drag-and-drop support in main window

## Tips & Tricks

- **Keep main window open** to quickly start multiple transfers
- **Open destination folders first** - they'll appear in the dropdown automatically
- **Use "Move" for large folders** - it's more efficient than copy+delete
- **Check "Apply to all"** when dealing with many file conflicts
- **Minimize progress windows** to keep your desktop organized during multiple transfers
- **Right-click in empty space** of a folder to quickly send something there
- **Speed in bits** is especially useful when transferring over network shares to compare against network bandwidth

## Contributing

Contributions are welcome! Please feel free to submit issues and pull requests.

### Development Setup
1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## Uninstallation

### Via Installer
- Open **Windows Settings** → **Apps** → **Installed apps**
- Search for "FastTransfer"
- Click the three dots → **Uninstall**
- Or run: `C:\Program Files\FastTransfer\Uninstall.exe`

### Manual Uninstall
- Delete the installation folder
- Remove registry entries (see manual installation section for keys)
- Refresh Explorer (log out/in or restart `explorer.exe`)

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

- Uses [Robocopy](https://docs.microsoft.com/en-us/windows-server/administration/windows-commands/robocopy) for folder operations
- Built with [Windows Forms](https://docs.microsoft.com/en-us/dotnet/desktop/winforms/) on .NET 8
- Installer created with [NSIS](https://nsis.sourceforge.io/)
- COM interop with Shell32 for Explorer integration

---

## Quick Start Example

```
1. Open File Explorer windows:
   - Window A: C:\Source\MyProject
   - Window B: D:\Backup

2. In Window A, right-click on "MyProject" folder
3. Select "FastTransfer"
4. Choose "Copy" operation
5. Select "D:\Backup" from dropdown
6. Click "Start"

7. Transfer starts immediately! Watch the progress:
   Initially:
   Copying folder...
   45.2 MB transferred
   12.3 MB/s (98.4 Mb/s)
   25 files transferred
   
   Once size is known:
   Copying folder...
   145.2 MB / 500 MB (29%)
   12.3 MB/s (98.4 Mb/s)
   25 / 100 files

8. Window auto-closes when done!
```

**Enjoy blazing-fast file transfers!**
