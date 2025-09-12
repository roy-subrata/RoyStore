/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./**/*.{razor,html,cshtml}",          // Current project
    "../Client/**/*.{razor,html,cshtml}",  // Other projects
    "../Client.Web/**/*.{razor,html,cshtml}" // Other projects
  ],
  theme: {
    extend: {},
  },
  plugins: [],
};
