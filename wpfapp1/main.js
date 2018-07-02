const electron = require('electron')

let mainWindow

function open(){
	console.log("open")
}
function createWindow () {

  mainWindow = new electron.BrowserWindow({width: 800, height: 600})
  
  
var menu = electron.Menu.buildFromTemplate([
    {
        label: 'File',
        submenu: [
			{
				label: 'Open',
				click: function() {
					open();
				}
			},
			{
				label: 'Play',
				click: open
			}
        ]
    }
]);
electron.Menu.setApplicationMenu(menu);

  // mainWindow.loadFile('index.html')
  // mainWindow.webContents.openDevTools()

  mainWindow.on('closed', function () {
    mainWindow = null
  })
}


electron.app.on('ready', createWindow)

electron.app.on('window-all-closed', function () {
  if (process.platform !== 'darwin') {
    electron.app.quit()
  }
})

electron.app.on('activate', function () {
  if (mainWindow === null) {
    createWindow()
  }
})