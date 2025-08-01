# Blare-Ng
A full-stack music streaming app built with Angular, .NET, and MongoDB.

[Screenshot](https://raw.githubusercontent.com/zackmorgs/BLARE-ng/refs/heads/master/screenshot.png)

### Showcase Videos
- [Authentication Demo](https://youtu.be/pwUMDN9jguI)
- [Tag Search Demo](https:/youtu.be/3xKZGKWNLLA)
- [Player Demo with State](https://youtu.be/HCnFgaB2jvw) 
- [Starred and Play/Pause](https://youtu.be/GwmdDOCC_nE)
- [Uploading Releases](https://youtu.be/wWa3FLSop_Y)
- [Streaming Demo](https://youtu.be/hBh27IDKRNc)
- [Track Options (play, share, star) Demo](https://youtu.be/3oY5uEt-DWs)

---

## Overview
Blare-Ng is a modern music streaming platform designed to showcase a robust, scalable stack for real-world production use. It features user authentication, artist/track/release management, file uploads, and a (for now) mobile-only Angular frontend. The backend is built with ASP.NET Core and MongoDB, and includes a custom tag scraper for music genres.

---

## Stack
- **Backend:**
  - C# .NET 9 REST API
  - MongoDB Atlas (cloud database)
  - JWT authentication (System.IdentityModel.Tokens.Jwt)
  - BCrypt.Net-Next for password hashing
  - File upload support (album art, WAV files)
    - In-progress right now
  - Will use [ATL](https://github.com/Zeugma440/atldotnet) for audio metadata
  
- **Frontend:**
  - Angular 17 (TypeScript)
  - SCSS styling (custom, no UI library)
  - RxJS for state management
- **Utilities:**
  - PuppeteerSharp-based tag scraper (`./src/tagScraper`) to populate the database with music genre tags

---

## Features
- **Authentication:**
  - Register, login, logout (JWT-based)
  - Role-based access (listener, artist, admin)
- **Release Management:**
  - Artists can create, edit, and delete releases
    - Single, EP, Album
  - File upload for album art and multiple WAV tracks
    - In progress
  - Releases and tracks are stored in MongoDB
- **Tag Scraper:**
  - Console app scrapes Wikipedia for music genres and populates the `tags` collection
  - uses wikipedia page to get list of tags
    - [link to page](https://en.wikipedia.org/wiki/List_of_music_genres_and_styles)
- **Responsive UI:**
  - Conditional navigation and content based on user role and authentication
- **API:**
  - RESTful endpoints for users, releases, tracks, and tags
  - CORS enabled for Angular dev server
- **Routing**
  - Slug-based navigation
  
---

## Getting Started

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- [Node.js & npm](https://nodejs.org/)
- [MongoDB Atlas account](https://www.mongodb.com/cloud/atlas)

### Backend Setup
**Note**: You cannot use my database unless you have whitelisted ip address.

1. Clone the repo and navigate to the root directory.
2. Update your MongoDB connection string in `src/Api/appsettings.json` and `src/tagScraper/appsettings.json`.
3. Build and run the backend:
   ```sh
   cd src/Api
   dotnet build
   dotnet run
   ```
   The API will be available at `https://localhost:7109` (or as configured).

### Frontend Setup
1. Navigate to the Angular client:
   ```
   cd src/ngClient
   npm install
   npm start
   ```
   The app will be available at `http://localhost:4200`.

### Tag Scraper
1. To populate the database with music genre tags:
   ```
   cd src/tagScraper
   dotnet run
   ```
   This will scrape Wikipedia and insert tags into the `tags` collection in MongoDB.

---

## Usage
- Register as a listener or artist, or use the demo accounts below.
- Artists can create and manage releases, upload album art and tracks.
- Listeners can browse and stream music (feature in progress).

### Demo Accounts
- **Listener**
  - Username: `user`
  - Password: `password1234`
- **Artist**
  - Username: `indie-band`
  - Password: `password1234`
- **Admin**
  - Not available yet

---

## Project Structure
```
BLARE-ng/
├── src/
│   ├── ngClient     # Angular spa client
│   ├── Api/         # .NET API (controllers, services, Program.cs)
│   ├── Data/        # MongoDB DataContext
│   ├── Models/      # C# models (User, Release, Track, etc.)
│   └── tagScraper/  # PuppeteerSharp console app for tags
     # Angular frontend
└── README.md
```

---

## Roadmap / To Do
- [ ] Integrate Google Cloud Platform (GCP)
- [ ] Storybook for UI components
- [ ] Nx monorepo support
- [ ] Dockerization
- [ ] Azure DevOps CI/CD
- [ ] Cypress end-to-end testing
- [ ] Admin dashboard development, among other required UI needed
- [ ] Streaming audio player
- [ ] Move PuppeteerSharp to straight Puppeteer in JavaScript - PuppeteerSharp wont work in deployment environment.

## URL Structure Goals
- api/play/{artist-slug}/{release-slug}
- api/play/{user}/playlist/{slug}
- GET
  - /api/{user}/playlists
    - gets all playlist for user (AUTHENTICATED)
- POST
  - /api/{user}/playlists
    - User creates a playlist
- GET
  - /api/user/{id}/playlist
- GET
  ~~- /api/{user}/playlist/{slug}~~
  ~~  - Get a user's playlist via slug name~~
  - This is problematic - would have to update slugs in db. Id doesn't change. I want this up and running.
- PUT
  - /api/{user}/playlist/{slug}
    - Update a user's playlist via slug
- DELETE
  - /api/{user}/playlist/{slug}
    - Delete a user's playlist