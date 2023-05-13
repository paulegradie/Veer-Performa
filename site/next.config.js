const withMarkdoc = require('@markdoc/next.js')

/** @type {import('next').NextConfig} */
const nextConfig = {
  basePath: '/github-pages',
  reactStrictMode: true,
  pageExtensions: ['js', 'jsx', 'md'],
  experimental: {
    scrollRestoration: true,
  },
  images: {
    loader: 'akamai',
    path: '',
  },
}

module.exports = withMarkdoc()(nextConfig)