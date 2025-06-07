import { Injectable } from '@angular/core';
import { Role } from '../../models/shared/enums/role.enum';

@Injectable({ providedIn: 'root' })
export class RoleService {
  private roleTranslations: Record<Role, string> = {
    [Role.Guest]: 'Гость',
    [Role.Gambler]: 'Игрок',
    [Role.Moderator]: 'Модератор',
    [Role.Bookmaker]: 'Букмекер',
    [Role.Administrator]: 'Администратор'
  };

  getTranslatedRoles(exclude: Role[] = [Role.Guest]): { value: Role; label: string }[] {
    return Object.values(Role)
      .filter(role => !exclude.includes(role))
      .map(role => ({
        value: role,
        label: this.roleTranslations[role] || role
      }));
  }

  translateRole(role: Role): string {
    return this.roleTranslations[role] || role;
  }
}