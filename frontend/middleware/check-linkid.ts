export default defineNuxtRouteMiddleware((to, from) => {
  const route = useRoute()

  if (!route.query.linkId) {
    return navigateTo('/')
  }
})
