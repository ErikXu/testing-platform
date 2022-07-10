import request from '@/utils/request'

export function getScheduleList() {
  return request({
    url: '/api/schedules',
    method: 'get'
  })
}

export function switchDisabled(id) {
  return request({
    url: `/api/schedules/${id}/disabled`,
    method: 'patch'
  })
}
