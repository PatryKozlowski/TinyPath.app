import type { $Fetch, NitroFetchRequest } from 'nitropack'
import type { CheckoutResponse, LinksResponse } from '~/types'

const LOGIN_ENDPOINT_API = '/api/User/LoginCommand'
const REGISTER_ENDPOINT_API = '/api/User/CreateUserCommand'
const LOADED_USER_DATA_ENDPOINT_API = '/api/User/GetAuthenticatedUserCommand'
const LOGOUT_USER_ENDPOINT_API = '/api/User/Logout'
const CREATE_LINK_ENDPOINT_API = '/api/Link/CreateShortLinkCommand'
const CREATE_CUSTOM_LINK_ENDPOINT_API = '/api/Link/CreateCustomShortLinkCommand'
const CREATE_LINK_GUEST_ENDPOINT_API = '/api/Link/CreateShortLinkCommand'
const GUEST_LINKS_ENDPOINT_API = '/api/Guest/GetGuestCommand'
const GUEST_EMAIL_ENDPOINT_API =
  '/api/Link/SendLinkViewsEmailForGuestUserCommand'
const CHECKOUT_ENDPOINT_API = '/api/Stripe/CreateCheckoutSession'
const GET_LINKS_ENDPOINT_API = '/api/Link/GetLinksCommand'
const GET_LINK_ENDPOINT_API = '/api/Link/GetLinkCommand'
const DELETE_LINK_ENDPOINT_API = '/api/Link/DeleteLinkCommand'
const UPDATE_LINK_ENDPOINT_API = '/api/Link/UpdateLinkCommand'

export const repository = <T>(fetch: $Fetch<T, NitroFetchRequest>) => ({
  async login(formValue: LoginForm): Promise<LoginResponse> {
    return await fetch<LoginResponse>(LOGIN_ENDPOINT_API, {
      method: 'POST',
      body: {
        email: formValue.email,
        password: formValue.password
      }
    })
  },

  async register(formValue: RegisterForm) {
    return await fetch<RegisterForm>(REGISTER_ENDPOINT_API, {
      method: 'POST',
      body: {
        email: formValue.email,
        password: formValue.password,
        repeatPassword: formValue.repeatPassword
      }
    })
  },

  async loadedUserData(): Promise<LoadedUserDataResponse> {
    return await fetch<LoadedUserDataResponse>(LOADED_USER_DATA_ENDPOINT_API, {
      method: 'GET'
    })
  },

  async logout() {
    return await fetch<LogoutResponse>(LOGOUT_USER_ENDPOINT_API, {
      method: 'POST'
    })
  },

  async createShortLink(createLink: CreateLinkForm) {
    return await fetch<CreateLinkResponse>(CREATE_LINK_ENDPOINT_API, {
      method: 'POST',
      body: {
        url: createLink.url,
        title: createLink.title
      }
    })
  },

  async createShortLinkForGuest(createLink: CreateLinkForm) {
    return await fetch<CreateLinkResponse>(CREATE_LINK_GUEST_ENDPOINT_API, {
      method: 'POST',
      body: {
        url: createLink.url,
        title: createLink.title
      }
    })
  },

  async createCustomShortLink(createLink: CreateCustomLinkForm) {
    return await fetch<CreateLinkResponse>(CREATE_CUSTOM_LINK_ENDPOINT_API, {
      method: 'POST',
      body: {
        url: createLink.url,
        title: createLink.title,
        customLink: createLink.customCode
      }
    })
  },

  async loadGuestData() {
    return await fetch<GuestData>(GUEST_LINKS_ENDPOINT_API, {
      method: 'GET'
    })
  },

  async sendEmailForTrackingLink(guestEmail: GuestEmailForm) {
    return await fetch(GUEST_EMAIL_ENDPOINT_API, {
      method: 'POST',
      body: {
        email: guestEmail.email
      }
    })
  },

  async createCheckoutSession(checkoutForm: CheckoutForm) {
    return await fetch<CheckoutResponse>(CHECKOUT_ENDPOINT_API, {
      method: 'POST',
      body: {
        priceCode: checkoutForm.priceCode
      }
    })
  },

  async loadLinksData(pageNo: number, pageSize: number) {
    return await fetch<LinksResponse>(
      `${GET_LINKS_ENDPOINT_API}?PageNo=${pageNo}&PageSize=${pageSize}`,
      {
        method: 'GET'
      }
    )
  },

  async loadLinkData(linkId: string) {
    return await fetch<LinkResponse>(
      `${GET_LINK_ENDPOINT_API}?LinkId=${linkId}`,
      {
        method: 'GET'
      }
    )
  },

  async deleteLink(linkId: string) {
    return await fetch<DeleteLinkResponse>(
      `${DELETE_LINK_ENDPOINT_API}?LinkId=${linkId}`,
      {
        method: 'DELETE'
      }
    )
  },

  async updateLink(linkId: string, editLink: EditLinkForm) {
    return await fetch<DeleteLinkResponse>(UPDATE_LINK_ENDPOINT_API, {
      method: 'PATCH',
      body: {
        linkId: linkId,
        title: editLink.title,
        active: editLink.active
      }
    })
  }
})
