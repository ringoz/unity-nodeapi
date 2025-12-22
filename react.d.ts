/**********************************************************************
 Copyright (c) Vladimir Davidovich. All rights reserved.
***********************************************************************/

import React from 'react';

export declare function createRoot(view: any): {
  render: (component: React.ReactNode) => Promise<void>;
  unmount: () => Promise<void>;
};
