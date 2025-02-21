# TimeGhazi

TimeGhazi is a shift management system designed for businesses to manage employee work schedules. The system consists of a web-based admin panel built with **ASP.NET Core MVC** and a mobile application built with **React Native**. Employees can view their assigned shifts in real-time using **SignalR** for instant updates.

## Features
### ✅ Admin Panel (ASP.NET Core MVC)
- Create, edit, and delete employee shifts
- Assign shifts to specific employees
- Real-time updates using SignalR
- Authentication and role-based access control

### 📱 Mobile Application (React Native)
- Employees can view their assigned shifts
- Live updates when shifts are created or modified
- Secure login with authentication

## Technologies Used
### Backend
- **ASP.NET Core MVC** - Web framework for the admin panel
- **Entity Framework Core** - ORM for database management
- **SQLite** - Lightweight database for storing shift data
- **ASP.NET Identity** - User authentication and authorization
- **SignalR** - Real-time communication for shift updates

### Frontend
- **React Native** - Cross-platform mobile application development
- **Expo** - Framework for easier React Native development
- **Axios** - HTTP client for API requests

### DevOps
- **Git** - Version control
- **Rider** - IDE for backend development
- **WebStorm** - IDE for frontend development

## Installation and Setup
### 🚀 Backend (Admin Panel)
1. Clone the repository:
   ```sh
   git clone https://github.com/your-repo/timeGhazi.git
   cd timeghazi/TimeGhazi
   ```
2. Install dependencies:
   ```sh
   dotnet restore
   ```
3. Apply database migrations:
   ```sh
   dotnet ef database update
   ```
4. Run the server:
   ```sh
   dotnet run
   ```
5. Open `http://localhost:5026/swagger` to test API endpoints.

### 📱 Mobile App
1. Navigate to the mobile directory:
   ```sh
   cd timeghazi-mobile
   ```
2. Install dependencies:
   ```sh
   npm install
   ```
3. Start the development server:
   ```sh
   npx expo start
   ```
4. Open the app in an emulator or physical device.

## API Endpoints
### Authentication
- `POST /api/auth/login` - User login

### Shifts
- `GET /api/shifts/{employeeId}` - Get shifts for a specific employee
- `POST /api/shifts` - Create a new shift (Admin only)

## Troubleshooting
### WebSocket Issues
- Ensure the backend is running with `dotnet run`.
- Check if WebSockets are allowed in firewall settings.
- Use `adb reverse tcp:5026 tcp:5026` if running on an Android emulator.

### Database Issues
- If migrations fail, try:
- First delete the DB and migrations files, then try:
   ```sh
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

